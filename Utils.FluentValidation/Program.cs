using System;
using System.Diagnostics;
using System.Linq;
using FluentValidation.Results;
using Utils.FluentValidation.Entity;
using Utils.FluentValidation.Validator;

namespace Utils.FluentValidation
{
    class Program
    {
        static void Main(string[] args)
        {
            // Valid Person
            Person pValid = new Person()
            {
                FirstName = "Jan",
                LastName = "Novák",
                DateOfBirth = new DateTime(1993, 1, 1),
                ReferenceToAnotherClass = new SpecialProperty()
                {
                    Property = "some value"
                }
            };
            // Not valid Person because of null in ReferenceToAnotherClass
            Person pNotValid = new Person()
            {
                FirstName = "Jan",
                LastName = "Novák",
                DateOfBirth = new DateTime(1993, 8, 24),
                ReferenceToAnotherClass = null // null is against Validator rules
            };

            // Not valid Person because of empty value in ReferenceToAnotherClass
            Person pNotValidReferenceToAnotherClass = new Person()
            {
                FirstName = "Jan",
                LastName = "Novák",
                DateOfBirth = new DateTime(1993, 8, 24),
                ReferenceToAnotherClass = new SpecialProperty()
                {
                    Property = ""
                }
            };

            // Print persons
            Console.WriteLine(pValid);
            Console.WriteLine(pNotValid);
            Console.WriteLine(pNotValidReferenceToAnotherClass);

            // Create instance of validator
            PersonValidator personValidator = new PersonValidator();

            Console.WriteLine();
            Console.WriteLine("pValid person results:");

            Validate(pValid, personValidator); // Ok

            Console.WriteLine();
            Console.WriteLine("pNotValid person results:");

            Validate(pNotValid, personValidator); // Error

            Console.WriteLine();
            Console.WriteLine("pNotValidReferenceToAnotherClass person results:");

            Validate(pNotValidReferenceToAnotherClass, personValidator); // Error

            // Valid only single property via reflection
            ValidationResult results3 = ValidationExtensions.Validate(personValidator, pValid, "SomeSpecialProperty");
            ValidationResult results4 = ValidationExtensions.Validate(personValidator, pNotValid, "SomeSpecialProperty");

        }

        /// <summary>
        /// Validate given person with its validator
        /// </summary>
        /// <param name="p">Person</param>
        /// <param name="validator">Person validator</param>
        static void Validate(Person p, PersonValidator validator)
        {
            ValidationResult results = validator.Validate(p);
            if (results.IsValid)
            {
                Console.WriteLine("ok");
            }
            else
            {
                results.Errors.ToList().ForEach(i => Console.WriteLine(i.ToString()));
            }
        }
    }
}
