#region Using namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Hosting;
using BusinessEntities;
using BusinessServices;
using DataModel;
using DataModel.GenericRepository;
using DataModel.UnitOfWork;
using Moq;
using NUnit.Framework;
using Newtonsoft.Json;
using TestsHelper;
using WebApi.Controllers;
using WebApi.ErrorHelper;
#endregion

namespace ApiController.Tests
{
    [TestFixture]
    public class ContactControllerTest
    {

        #region Variables

        private IContactServices _contactService;
        private ITokenServices _tokenService;
        private IUnitOfWork _unitOfWork;
        private List<Contact> _contacts;
        private List<Tokens> _tokens;
        private GenericRepository<Contact> _contactRepository;
        private GenericRepository<Tokens> _tokenRepository;
        private WebApiDbEntities _dbEntities;
        private HttpClient _client;

        private HttpResponseMessage _response;
        private string _token;
        private const string ServiceBaseURL = "http://localhost:50876/";

        #endregion

        #region Test fixture setup

        /// <summary>
        /// Initial setup for tests
        /// </summary>
        [TestFixtureSetUp]
        public void Setup()
        {
            _contacts = SetUpContacts();
            _tokens = SetUpTokens();
            _dbEntities = new Mock<WebApiDbEntities>().Object;
            _tokenRepository = SetUpTokenRepository();
            _contactRepository = SetUpContactRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.ContactRepository).Returns(_contactRepository);
            unitOfWork.SetupGet(s => s.TokenRepository).Returns(_tokenRepository);
            _unitOfWork = unitOfWork.Object;
            _contactService = new ContactServices(_unitOfWork);
            _tokenService = new TokenServices(_unitOfWork);
            _client = new HttpClient { BaseAddress = new Uri(ServiceBaseURL) };
            var tokenEntity = _tokenService.GenerateToken(1);
            _token = tokenEntity.AuthToken;
            _client.DefaultRequestHeaders.Add("Token", _token);
        }

        #endregion

        #region Setup
        /// <summary>
        /// Re-initializes test.
        /// </summary>
        [SetUp]
        public void ReInitializeTest()
        {
            _client = new HttpClient { BaseAddress = new Uri(ServiceBaseURL) };
            _client.DefaultRequestHeaders.Add("Token", _token);
        }
        #endregion

        #region Private member methods

        /// <summary>
        /// Setup dummy repository
        /// </summary>
        /// <returns></returns>
        private GenericRepository<Contact> SetUpContactRepository()
        {
            // Initialise repository
            var mockRepo = new Mock<GenericRepository<Contact>>(MockBehavior.Default, _dbEntities);

            // Setup mocking behavior
            mockRepo.Setup(p => p.GetAll()).Returns(_contacts);

            mockRepo.Setup(p => p.GetByID(It.IsAny<int>()))
                .Returns(new Func<int, Contact>(
                             id => _contacts.Find(p => p.Id.Equals(id))));

            mockRepo.Setup(p => p.Insert((It.IsAny<Contact>())))
                .Callback(new Action<Contact>(newContact =>
                {
                    dynamic maxContactID = _contacts.Last().Id;
                    dynamic nextContactID = maxContactID + 1;
                    newContact.Id = nextContactID;
                    _contacts.Add(newContact);
                }));

            mockRepo.Setup(p => p.Update(It.IsAny<Contact>()))
                .Callback(new Action<Contact>(prod =>
                {
                    var oldContact = _contacts.Find(a => a.Id == prod.Id);
                    oldContact = prod;
                }));

            mockRepo.Setup(p => p.Delete(It.IsAny<Contact>()))
                .Callback(new Action<Contact>(prod =>
                {
                    var contactToRemove =
                        _contacts.Find(a => a.Id == prod.Id);

                    if (contactToRemove != null)
                        _contacts.Remove(contactToRemove);
                }));

            // Return mock implementation object
            return mockRepo.Object;
        }

        /// <summary>
        /// Setup dummy repository
        /// </summary>
        /// <returns></returns>
        private GenericRepository<Tokens> SetUpTokenRepository()
        {
            // Initialise repository
            var mockRepo = new Mock<GenericRepository<Tokens>>(MockBehavior.Default, _dbEntities);

            // Setup mocking behavior
            mockRepo.Setup(p => p.GetAll()).Returns(_tokens);

            mockRepo.Setup(p => p.GetByID(It.IsAny<int>()))
                .Returns(new Func<int, Tokens>(
                             id => _tokens.Find(p => p.TokenId.Equals(id))));

            mockRepo.Setup(p => p.Insert((It.IsAny<Tokens>())))
                .Callback(new Action<Tokens>(newToken =>
                {
                    dynamic maxTokenID = _tokens.Last().TokenId;
                    dynamic nextTokenID = maxTokenID + 1;
                    newToken.TokenId = nextTokenID;
                    _tokens.Add(newToken);
                }));

            mockRepo.Setup(p => p.Update(It.IsAny<Tokens>()))
                .Callback(new Action<Tokens>(token =>
                {
                    var oldToken = _tokens.Find(a => a.TokenId == token.TokenId);
                    oldToken = token;
                }));

            mockRepo.Setup(p => p.Delete(It.IsAny<Tokens>()))
                .Callback(new Action<Tokens>(prod =>
                {
                    var tokenToRemove =
                        _tokens.Find(a => a.TokenId == prod.TokenId);

                    if (tokenToRemove != null)
                        _tokens.Remove(tokenToRemove);
                }));

            // Return mock implementation object
            return mockRepo.Object;
        }

        /// <summary>
        /// Setup dummy contacts data
        /// </summary>
        /// <returns></returns>
        private static List<Contact> SetUpContacts()
        {
            var prodId = new int();
            var contacts = DataInitializer.GetAllContact();
            foreach (Contact prod in contacts)
                prod.Id = ++prodId;
            return contacts;
        }

        /// <summary>
        /// Setup dummy tokens data
        /// </summary>
        /// <returns></returns>
        private static List<Tokens> SetUpTokens()
        {
            var tokId = new int();
            var tokens = DataInitializer.GetAllTokens();
            foreach (Tokens tok in tokens)
                tok.TokenId = ++tokId;
            return tokens;
        }

        #endregion

        #region Unit Tests

        /// <summary>
        /// Get all contacts test
        /// </summary>
        [Test]
        public void GetAllContactsTest()
        {
            var contactController = new ContactsController(_contactService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseURL + "v1/Contacts/Contact/all")
                }
            };
            contactController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            _response = contactController.Get();

            var responseResult = JsonConvert.DeserializeObject<List<Contact>>(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(responseResult.Any(), true);
            var comparer = new ContactComparer();
            CollectionAssert.AreEqual(
                responseResult.OrderBy(contact => contact, comparer),
                _contacts.OrderBy(contact => contact, comparer), comparer);
        }

        /// <summary>
        /// Get contact by Id
        /// </summary>
        [Test]
        public void GetContactByIdTest()
        {
            var contactController = new ContactsController(_contactService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseURL + "v1/Contacts/Contact/1")
                }
            };
            contactController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            _response = contactController.Get(1);

            var responseResult = JsonConvert.DeserializeObject<Contact>(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            AssertObjects.PropertyValuesAreEquals(responseResult,
                                                    _contacts.Find(a => a.Firstname.Contains("Dipesh")));
        }

        /// <summary>
        /// Get contact by wrong Id
        /// </summary>
        [Test]
        //[ExpectedException("WebApi.ErrorHelper.ApiDataException")]
        public void GetContactByWrongIdTest()
        {
            var contactController = new ContactsController(_contactService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseURL + "v1/Contacts/Contact/contactid/10")
                }
            };
            contactController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var ex = Assert.Throws<ApiDataException>(() => contactController.Get(10));
            Assert.That(ex.ErrorCode, Is.EqualTo(1001));
            Assert.That(ex.ErrorDescription, Is.EqualTo("No contact found for this id."));

        }

        /// <summary>
        /// Get contact by invalid Id
        /// </summary>
        [Test]
        // [ExpectedException("WebApi.ErrorHelper.ApiException")]
        public void GetContactByInvalidIdTest()
        {
            var contactController = new ContactsController(_contactService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseURL + "v1/Contacts/Contact/contactid/-1")
                }
            };
            contactController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var ex = Assert.Throws<ApiException>(() => contactController.Get(-1));
            Assert.That(ex.ErrorCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(ex.ErrorDescription, Is.EqualTo("Bad Request..."));
        }

        /// <summary>
        /// Create contact test
        /// </summary>
        [Test]
        public void CreateContactTest()
        {
            var contactController = new ContactsController(_contactService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(ServiceBaseURL + "v1/Contacts/Contact/Create")
                }
            };
            contactController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var newContact = new ContactEntity()
            {
                firstName = "Michel",
                 lastName ="Baney",
                  email = "em@em.com"
            };

            var maxContactIDBeforeAdd = _contacts.Max(a => a.Id);
            newContact.id = maxContactIDBeforeAdd + 1;
            contactController.Post(newContact);
            var addedcontact = new Contact() { Firstname = newContact.firstName, Id = newContact.id , Email = newContact.email, Lastname = newContact.lastName};
            AssertObjects.PropertyValuesAreEquals(addedcontact, _contacts.Last());
            Assert.That(maxContactIDBeforeAdd + 1, Is.EqualTo(_contacts.Last().Id));
        }

        /// <summary>
        /// Update contact test
        /// </summary>
        [Test]
        public void UpdateContactTest()
        {
            var contactController = new ContactsController(_contactService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(ServiceBaseURL + "v1/Contacts/Contact/Modify")
                }
            };
            contactController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var firstContact = _contacts.First();
            firstContact.Lastname = "Parekh1 updated";
            var updatedContact = new ContactEntity() { lastName = firstContact.Lastname,  id = firstContact.Id };
            contactController.Put(firstContact.Id, updatedContact);
            Assert.That(firstContact.Id, Is.EqualTo(1)); // hasn't changed
        }

        /// <summary>
        /// Delete contact test
        /// </summary>
        [Test]
        public void DeleteContactTest()
        {
            var contactController = new ContactsController(_contactService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(ServiceBaseURL + "v1/Contacts/Contact/Remove")
                }
            };
            contactController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            int maxID = _contacts.Max(a => a.Id); // Before removal
            var lastContact = _contacts.Last();

            // Remove last Contact
            contactController.Delete(lastContact.Id);
            Assert.That(maxID, Is.GreaterThan(_contacts.Max(a => a.Id))); // Max id reduced by 1
        }

        /// <summary>
        /// Delete contact test with invalid id
        /// </summary>
        [Test]
        public void DeleteContactInvalidIdTest()
        {
            var contactController = new ContactsController(_contactService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(ServiceBaseURL + "v1/Contacts/Contact/remove")
                }
            };
            contactController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var ex = Assert.Throws<ApiException>(() => contactController.Delete(-1));
            Assert.That(ex.ErrorCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(ex.ErrorDescription, Is.EqualTo("Bad Request..."));
        }

        /// <summary>
        /// Delete contact test with wrong id
        /// </summary>
        [Test]
        public void DeleteContactWrongIdTest()
        {
            var contactController = new ContactsController(_contactService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(ServiceBaseURL + "v1/Contacts/Remove")
                }
            };
            contactController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            int maxID = _contactService.GetAllContact().Count(); //  _contacts.Max(a => a.Id); // Before removal

            var ex = Assert.Throws<ApiDataException>(() => contactController.Delete(maxID + 1));
            Assert.That(ex.ErrorCode, Is.EqualTo(1002));
            Assert.That(ex.ErrorDescription, Is.EqualTo("contact is already deleted or not exist in system."));
        }

        #endregion

        #region Integration Test

        /// <summary>
        /// Get all contacts test
        /// </summary>
        [Test]
        public void GetAllContactsIntegrationTest()
        {
            #region To be written inside Setup method specifically for integration tests
            var client = new HttpClient { BaseAddress = new Uri(ServiceBaseURL) };
            client.DefaultRequestHeaders.Add("Authorization", "Basic YWtoaWw6YWtoaWw=");
            MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
            _response = client.PostAsync("login", null).Result;

            if (_response != null && _response.Headers != null && _response.Headers.Contains("Token") && _response.Headers.GetValues("Token") != null)
            {
                client.DefaultRequestHeaders.Clear();
                _token = ((string[])(_response.Headers.GetValues("Token")))[0];
                client.DefaultRequestHeaders.Add("Token", _token);
            }
            #endregion

            _response = client.GetAsync("v1/Contacts/Contact/allcontacts/").Result;
            var responseResult =
                JsonConvert.DeserializeObject<List<ContactEntity>>(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(responseResult.Any(), true);
        }

        #endregion

        #region Tear Down
        /// <summary>
        /// Tears down each test data
        /// </summary>
        [TearDown]
        public void DisposeTest()
        {
            if (_response != null)
                _response.Dispose();
            if (_client != null)
                _client.Dispose();
        }

        #endregion

        #region TestFixture TearDown.

        /// <summary>
        /// TestFixture teardown
        /// </summary>
        [TestFixtureTearDown]
        public void DisposeAllObjects()
        {
            _tokenService = null;
            _contactService = null;
            _unitOfWork = null;
            _tokenRepository = null;
            _contactRepository = null;
            _tokens = null;
            _contacts = null;
            if (_response != null)
                _response.Dispose();
            if (_client != null)
                _client.Dispose();
        }

        #endregion
    }
}
