using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.FluentValidation
{
    class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public object SomeSpecialProperty { get; set; }
    }
}
