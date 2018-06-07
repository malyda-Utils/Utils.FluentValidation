using System;
using System.Diagnostics;
using System.Linq;
using FluentValidation.Results;
using Utils.FluentValidation.Entity;
using Utils.FluentValidation.FluentStyleExample;
using Utils.FluentValidation.Validator;

namespace Utils.FluentValidation
{
    class Program
    {
        static Person pValid;
        static Person pNotValid;
        static Person pNotValidReferenceToAnotherClass;
        static void Main(string[] args)
        {
            CreatePersonsViaBuilder();

            // Print persons
            Console.WriteLine("pValid:");
            Console.WriteLine(pValid);
            Console.WriteLine();

            Console.WriteLine("pNotValid:");
            Console.WriteLine(pNotValid);
            Console.WriteLine();

            Console.WriteLine("pNotValidReferenceToAnotherClass:");
            Console.WriteLine(pNotValidReferenceToAnotherClass);
            Console.WriteLine();
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
            // true
            ValidationResult results3 = ValidationExtensions.Validate(personValidator, pValid, "ReferenceToAnotherClass");
            
            // false
            ValidationResult results4 = ValidationExtensions.Validate(personValidator, pNotValid, "ReferenceToAnotherClass");

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
                Console.WriteLine("Valid");
            }
            else
            {
                results.Errors.ToList().ForEach(i => Console.WriteLine(i.ToString()));
            }
        }
        /// <summary>
        /// Creates persons via basic Builder pattern
        /// </summary>
        static void CreatePersonsViaBuilder()
        {
            // Valid Person
            PersonBuilder pValidPersonBuilder = new PersonBuilder()
                .WithFirstName("Jan")
                .WithLastName("Novák")
                .WithDateOfBirth(new DateTime(1993, 1, 1))
                .WithEmail("john.doe@email.com")
                .WithReferencedClass(new ReferencedClassFromPerson()
                {
                    Property = "some value"
                });

            pValid = pValidPersonBuilder.Build();

            // Not valid Person because of null in ReferenceToAnotherClass
            PersonBuilder pNotValidPersonBuilder = new PersonBuilder()
                .WithFirstName("Jan")
                .WithLastName("Novák")
                .WithDateOfBirth(new DateTime(1993, 1, 1))
                .WithEmail("john.doe@email.com")
                .WithReferencedClass(null);

            pNotValid = pNotValidPersonBuilder.Build();

            // Not valid Person because of empty value in ReferenceToAnotherClass
            PersonBuilder pNotValidReferenceToAnotherClassBuilder = new PersonBuilder()
                    .WithFirstName("Jan")
                    .WithLastName("Novák")
                    .WithDateOfBirth(new DateTime(1993, 1, 1))
                    .WithEmail("john.doe@email.com")
                    .WithReferencedClass(new ReferencedClassFromPerson()
                    {
                        Property = "some value"
                    });

            pNotValidReferenceToAnotherClass = pNotValidReferenceToAnotherClassBuilder.Build();
        }

        /// <summary>
        /// Creates persons via standard Object Initializer method
        /// </summary>
        static void CreatePersonsViaObjectInitializer()
        {
            // Valid Person
            pValid = new Person()
            {
                FirstName = "Jan",
                LastName = "Novák",
                Email = "john.doe@email.com",
                DateOfBirth = new DateTime(1993, 1, 1),
                ReferenceToAnotherClass = new ReferencedClassFromPerson()
                {
                    Property = "some value"
                }
            };
            // Not valid Person because of null in ReferenceToAnotherClass
            pNotValid = new Person()
            {
                FirstName = "Jan",
                LastName = "Novák",
                Email = "john.doe@email.com",
                DateOfBirth = new DateTime(1994, 1, 1),
                ReferenceToAnotherClass = null // null is against Validator rules
            };

            // Not valid Person because of empty value in ReferenceToAnotherClass
            pNotValidReferenceToAnotherClass = new Person()
            {
                FirstName = "Jan",
                LastName = "Novák",
                Email = "john.doe@email.com",
                DateOfBirth = new DateTime(1995, 1, 1),
                ReferenceToAnotherClass = new ReferencedClassFromPerson()
                {
                    Property = ""
                }
            };
        }
    }
}
