using System;
using System.Collections;
using System.Collections.Generic;
using DataModel;

namespace TestsHelper
{
    public class ContactComparer : IComparer, IComparer<Contact>
    {
        public int Compare(object expected, object actual)
        {
            var lhs = expected as Contact;
            var rhs = actual as Contact;
            if (lhs == null || rhs == null) throw new InvalidOperationException();
            return Compare(lhs, rhs);
        }

        public int Compare(Contact expected, Contact actual)
        {
            int temp;
            return (temp = expected.Id.CompareTo(actual.Id)) != 0 ? temp : expected.Firstname.CompareTo(actual.Firstname);
        }
    }


}
