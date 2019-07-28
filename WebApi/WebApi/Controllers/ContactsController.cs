using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AttributeRouting;
using AttributeRouting.Web.Http;
using BusinessEntities;
using BusinessServices;
using WebApi.ActionFilters;
using WebApi.Filters;
using System;
using WebApi.ErrorHelper;

namespace WebApi.Controllers
{
    [AuthorizationRequired]
    [RoutePrefix("v1/Contacts")]
    public class ContactsController : ApiController
    {
        #region Private variable.

        private readonly IContactServices _contactServices;

        #endregion

        #region Public Constructor

        /// <summary>
        /// Public constructor to initialize Contact service instance
        /// </summary>
        public ContactsController(IContactServices contactServices)
        {
            _contactServices = contactServices;
        }

        #endregion

        // GET api/Contact
        [GET("allContacts")]        
        [GET("All")]
        public HttpResponseMessage Get()
        {
            var contacts = _contactServices.GetAllContact();
            var contacEntities = contacts as List<ContactEntity> ?? contacts.ToList();
            if (contacEntities.Any())
                return Request.CreateResponse(HttpStatusCode.OK, contacEntities);
            throw new ApiDataException(1000, "Contact not found", HttpStatusCode.NotFound);
        }

        // GET api/contact/5
        
        [GET("Contact/{id?}")]
        [GET("particularcontact/{id?}")]
        [GET("mycontact/{id:range(1, 3)}")]
        public HttpResponseMessage Get(int id)
        {
            if (id != null && id >0)
            {
                var contact = _contactServices.GetContactById(id);
                if (contact != null)
                    return Request.CreateResponse(HttpStatusCode.OK, contact);

                throw new ApiDataException(1001, "No contact found for this id.", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }

        // POST api/contact
        [POST("Create")]
        [POST("Register")]
        public int Post([FromBody] ContactEntity contactEntity)
        {
            return _contactServices.CreateContact(contactEntity);
        }

        // PUT api/contact/5
        [PUT("Update/{id}")]
        [PUT("Modify/contactid/{id}")]
        public bool Put(int id, [FromBody] ContactEntity contactEntity)
        {
            if (id > 0)
            {
                return _contactServices.UpdateContact(id, contactEntity);
            }
            return false;
        }

        // DELETE api/contact/5
        [DELETE("Remove/{id}")]
        [DELETE("clear/contactid/{id}")]
        [PUT("Delete/{id}")]
        public bool Delete(int id)
        {
            if (id != null && id > 0)
            {
                var isSuccess = _contactServices.DeleteContact(id);
                if (isSuccess)
                {
                    return isSuccess;
                }
                throw new ApiDataException(1002, "contact is already deleted or not exist in system.", HttpStatusCode.NoContent);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }
    }
}
