using System;
using System.Collections.Generic;
using System.Text;
using Utils.FluentValidation.Entity;

namespace Utils.FluentValidation.FluentStyleExample
{
    class PersonBuilder
    {
        String email;
        String firstName;
        String lastName;
        DateTime dateOfBirth = new DateTime();
        ReferencedClassFromPerson referencedClassFromPerson;

        public PersonBuilder WithEmail(string email)
        {
            this.email = email;
            return this;
        }

        public PersonBuilder WithFirstName(string firstName)
        {
            this.firstName = firstName;
            return this;
        }

        public PersonBuilder WithLastName(string lastName)
        {
            this.lastName = lastName;
            return this;
        }

        public PersonBuilder WithDateOfBirth(DateTime dateOfBirth)
        {
            this.dateOfBirth = dateOfBirth;
            return this;
        }

        public PersonBuilder WithReferencedClass(ReferencedClassFromPerson reference)
        {
            this.referencedClassFromPerson = reference;
            return this;
        }

        public Person Build()
        {
            return new Person()
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                ReferenceToAnotherClass = referencedClassFromPerson
            };
        }

    }
}
