using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using BusinessEntities;

namespace BusinessServices
{
      public interface IContactServices
    {
        ContactEntity GetContactById(int id);
        IEnumerable<ContactEntity> GetAllContact();
        int CreateContact(ContactEntity contactEntity);
        bool UpdateContact(int id, ContactEntity contactEntity);
        bool DeleteContact(int id);
    }
}
 
