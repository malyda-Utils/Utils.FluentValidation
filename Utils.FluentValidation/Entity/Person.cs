using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.FluentValidation.Entity
{
    class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
 
        public ReferencedClassFromPerson ReferenceToAnotherClass { get; set; }

        public override string ToString()
        {
            return $"First name: {FirstName}, Last name: {LastName}, Email: {Email}, Date of birth: {DateOfBirth}, SomeSpecialProperty type of object: {ReferenceToAnotherClass}";
        }
    }
}
