using System;
using System.Collections.Generic;
using DataModel;

namespace TestsHelper
{
    /// <summary>
    /// Data initializer for unit tests
    /// </summary>
    public class DataInitializer
    {
         
        /// <summary>
        /// Dummy Contact
        /// </summary>
        /// <returns></returns>
        public static List<Contact> GetAllContact()
        {
            var contacts = new List<Contact>
                               {
                                   new Contact() { Firstname = "Dipesh", Lastname="Parekh" , Email="di@di.com"},
                                   new Contact() { Firstname = "Ket", Lastname="John" ,Email="Ket@ka.com"},
                                    new Contact() { Firstname = "John", Lastname="John" ,Email="Ket@ka.com" , Id =3},
                                     new Contact() { Firstname = "Disha", Lastname="John" ,Email="Ket@ka.com", Id =4},

                               };
            return contacts;
        }

        /// <summary>
        /// Dummy Contact
        /// </summary>
        /// <returns></returns>
        public static List<Project> GetAllProject()
        {
            var contacts = new List<Project>
                               {
                                   new Project() { ContactId=1, EndDate = DateTime.Now.AddMonths(2), IsSetDate = true, Priority =3 , ProjectName ="Test1", StartDate = DateTime.Now.AddMonths(1) },
                                   new Project() { ContactId=2, EndDate = DateTime.Now.AddMonths(4), IsSetDate = true, Priority =4 , ProjectName ="Test2", StartDate = DateTime.Now.AddMonths(1) },

                               };
            return contacts;
        }

        /// <summary>
        /// Dummy tokens
        /// </summary>
        /// <returns></returns>
        public static List<Tokens> GetAllTokens()
        {
            var tokens = new List<Tokens>
                               {
                                   new Tokens()
                                       {
                                           AuthToken = "9f907bdf-f6de-425d-be5b-b4852eb77761",
                                           ExpiresOn = DateTime.Now.AddHours(2),
                                           IssuedOn = DateTime.Now,
                                           UserId = 1
                                       },
                                   new Tokens()
                                       {
                                           AuthToken = "9f907bdf-f6de-425d-be5b-b4852eb77762",
                                           ExpiresOn = DateTime.Now.AddHours(1),
                                           IssuedOn = DateTime.Now,
                                           UserId = 2
                                       }
                               };
                               
            return tokens;
        }

        /// <summary>
        /// Dummy users
        /// </summary>
        /// <returns></returns>
        public static List<User> GetAllUsers()
        {
            var users = new List<User>
                               {
                                   new User()
                                       {
                                           UserName = "dipesh",
                                           Password = "dipesh",
                                           Name = "dipesh Parekh",
                                       },
                                   new User()
                                       {
                                           UserName = "Ajay",
                                           Password = "Ajay",
                                           Name = "Ajay Patel",
                                       }                                   
                               };

            return users;
        }

    }
}
