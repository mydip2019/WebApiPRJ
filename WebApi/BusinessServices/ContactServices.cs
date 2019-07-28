using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AutoMapper;
using BusinessEntities;
using DataModel;
using DataModel.UnitOfWork;

namespace BusinessServices
{
    /// <summary>
    /// Offers services for product specific CRUD operations
    /// </summary>
    public class ContactServices : IContactServices
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public ContactServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Fetches Contact details by id
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public BusinessEntities.ContactEntity GetContactById(int id)
        {
            var contact = _unitOfWork.ContactRepository.GetByID(id);
            if (contact != null)
            {
                Mapper.CreateMap<Contact, ContactEntity>();
                var contactModel = Mapper.Map<Contact, ContactEntity>(contact);
                return contactModel;
            }
            return null;
        }

        /// <summary>
        /// Fetches all the contacts.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BusinessEntities.ContactEntity> GetAllContact()
        {
            var contacts = _unitOfWork.ContactRepository.GetAll().ToList();
            if (contacts.Any())
            {
                Mapper.CreateMap<Contact, ContactEntity>();
                var contactsModel = Mapper.Map<List<Contact>, List<ContactEntity>>(contacts);
                return contactsModel;
            }
            return null;
        }

        /// <summary>
        /// Creates a contact
        /// </summary>
        /// <param name="productEntity"></param>
        /// <returns></returns>
        public int CreateContact(BusinessEntities.ContactEntity contactEntity)
        {
            using (var scope = new TransactionScope())
            {
                var contact = new Contact
                {
                    Firstname = contactEntity.firstName,
                     Email = contactEntity.email,
                      Lastname = contactEntity.lastName
                };
                _unitOfWork.ContactRepository.Insert(contact);
                _unitOfWork.Save();
                scope.Complete();
                return contact.Id;
            }
        }

        /// <summary>
        /// Updates a contact
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="contactEntity"></param>
        /// <returns></returns>
        public bool UpdateContact(int id, BusinessEntities.ContactEntity contactEntity)
        {
            var success = false;
            if (contactEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var contact = _unitOfWork.ContactRepository.GetByID(id);
                    if (contact != null)
                    {
                        contact.Firstname = contactEntity.firstName;
                        contact.Email = contactEntity.email;
                        contact.Lastname = contactEntity.lastName;
                        _unitOfWork.ContactRepository.Update(contact);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Deletes a particular contact
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteContact(int id)
        {
            var success = false;
            if (id > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var contact = _unitOfWork.ContactRepository.GetByID(id);
                    if (contact != null)
                    {

                        _unitOfWork.ContactRepository.Delete(contact);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

      
    }
}
