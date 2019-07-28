#region using namespaces.
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BusinessEntities;
using DataModel;
using DataModel.GenericRepository;
using DataModel.UnitOfWork;
using Moq;
using NUnit.Framework;
using TestsHelper;

#endregion

namespace BusinessServices.Tests
{
    /// <summary>
    /// Contact Service Test
    /// </summary>
    public class ContactServicesTest
    {
        #region Variables

        private IContactServices _contactServices;
        private IUnitOfWork _unitOfWork;
        private List<Contact> _contacts;
        private GenericRepository<Contact> _contactRepository;
        private WebApiDbEntities _dbEntities;
        #endregion

        #region Test fixture setup

        /// <summary>
        /// Initial setup for tests
        /// </summary>
        [TestFixtureSetUp]
        public void Setup()
        {
            _contacts = SetUpContacts();
        }

        #endregion

        #region Setup

        /// <summary>
        /// Re-initializes test.
        /// </summary>
        [SetUp]
        public void ReInitializeTest()
        {
            _contacts = SetUpContacts();
            _dbEntities = new Mock<WebApiDbEntities>().Object;
            _contactRepository = SetUpContactRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.ContactRepository).Returns(_contactRepository);
            _unitOfWork = unitOfWork.Object;
            _contactServices = new ContactServices(_unitOfWork);

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

        #endregion

        #region Unit Tests

        /// <summary>
        /// Service should return all the contacts
        /// </summary>
        [Test]
        public void GetAllContactsTest()
        {
            var contacts = _contactServices.GetAllContact();
            if (contacts != null)
            {
                var contactList =
                    contacts.Select(
                        contactEntity =>
                        new Contact {  Id = contactEntity.id,
                            Firstname = contactEntity.firstName,
                            Lastname =contactEntity.lastName,
                            Email = contactEntity.email }).
                        ToList();
                var comparer = new ContactComparer();
                CollectionAssert.AreEqual(
                    contactList.OrderBy(contact => contact, comparer),
                    _contacts.OrderBy(contact => contact, comparer), comparer);
            }
        }

        /// <summary>
        /// Service should return null
        /// </summary>
        [Test]
        public void GetAllContactsTestForNull()
        {
            _contacts.Clear();
            var contacts = _contactServices.GetAllContact();
            Assert.Null(contacts);
            SetUpContacts();
        }

        /// <summary>
        /// Service should return contact if correct id is supplied
        /// </summary>
        [Test]
        public void GetContactByRightIdTest()
        {
            var mobileContact = _contactServices.GetContactById(2);
            if (mobileContact != null)
            {
                Mapper.CreateMap<ContactEntity, Contact>();
                var contactModel = Mapper.Map<ContactEntity, Contact>(mobileContact);
                AssertObjects.PropertyValuesAreEquals(contactModel,_contacts.Find(a => a.Firstname.Contains("Ket")));
            }
        }

        /// <summary>
        /// Service should return null
        /// </summary>
        [Test]
        public void GetContactByWrongIdTest()
        {
            var contact = _contactServices.GetContactById(0);
            Assert.Null(contact);
        }

        /// <summary>
        /// Add new contact test
        /// </summary>
        [Test]
        public void AddNewContactTest()
        {
            var newContact = new ContactEntity()
            {
                firstName = "Britney",
                 lastName = "James",
                  email = "brit@tf.com"            
            };

            var maxContactIDBeforeAdd = _contacts.Max(a => a.Id);
            newContact.id = maxContactIDBeforeAdd + 1;
            _contactServices.CreateContact(newContact);
            var addedcontact = new Contact() { Firstname = newContact.firstName, Id = newContact.id, Email =newContact.email, Lastname = newContact.lastName };
            AssertObjects.PropertyValuesAreEquals(addedcontact, _contacts.Last());
            Assert.That(maxContactIDBeforeAdd + 1, Is.EqualTo(_contacts.Last().Id));
        }

        /// <summary>
        /// Update contact test
        /// </summary>
        [Test]
        public void UpdateContactLastNameTest()
        {
            var firstContact = _contacts.First();
            firstContact.Lastname = "Parekh1 updated";
            var updatedContact = new ContactEntity()
            { lastName = firstContact.Lastname, id = firstContact.Id };
            _contactServices.UpdateContact(firstContact.Id, updatedContact);
            Assert.That(firstContact.Id, Is.EqualTo(1)); // hasn't changed
            Assert.That(firstContact.Lastname, Is.EqualTo("Parekh1 updated")); // Contact name changed
        }

        /// <summary>
        /// Update contact test
        /// </summary>
        [Test]
        public void UpdateContactTest()
        {
            var firstContact = _contacts.First();
            firstContact.Lastname = "Parekh1 updated";
            firstContact.Firstname = "DipeshNew updated";
            firstContact.Email = "dinew@di.com";
            var updatedContact = new ContactEntity()
            { lastName = firstContact.Lastname, id = firstContact.Id , email = firstContact.Email, firstName = firstContact.Firstname};
            _contactServices.UpdateContact(firstContact.Id, updatedContact);
            Assert.That(firstContact.Id, Is.EqualTo(1)); // hasn't changed
            Assert.That(firstContact.Lastname, Is.EqualTo("Parekh1 updated")); // Contact last name changed
            Assert.That(firstContact.Firstname, Is.EqualTo("DipeshNew updated")); // Contact first name changed
            Assert.That(firstContact.Email, Is.EqualTo("dinew@di.com")); // Contact email changed
        }


        /// <summary>
        /// Delete contact test
        /// </summary>
        [Test]
        public void DeleteContactTest()
        {
            int startMaxID = _contactServices.GetAllContact().Count();
           // _contacts.Max(a => a.Id); // Before removal
            var lastContact = _contacts.Last();

            // Remove last Contact
            _contactServices.DeleteContact(lastContact.Id);
            int endMaxID = _contactServices.GetAllContact().Count();
            //Assert.That(maxID, Is.GreaterThan(_contacts.Max(a => a.Id))); // Max id reduced by 1
            Assert.IsTrue(endMaxID == (startMaxID - 1));
        }

        #endregion


        #region Tear Down

        /// <summary>
        /// Tears down each test data
        /// </summary>
        [TearDown]
        public void DisposeTest()
        {
            _contactServices = null;
            _unitOfWork = null;
            _contactRepository = null;
            if (_dbEntities != null)
                _dbEntities.Dispose();
            _contacts = null;
        }

        #endregion

        #region TestFixture TearDown.

        /// <summary>
        /// TestFixture teardown
        /// </summary>
        [TestFixtureTearDown]
        public void DisposeAllObjects()
        {
            _contacts = null;
        }

        #endregion
    }
}
