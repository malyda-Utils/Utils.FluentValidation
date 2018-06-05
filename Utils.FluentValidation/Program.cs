using System;
using System.Diagnostics;
using System.Linq;
using FluentValidation.Results;

namespace Utils.FluentValidation
{
    class Program
    {
        static void Main(string[] args)
        {
            Person pValid = new Person()
            {
                FirstName = "Jan",
                LastName = "Novák",
                DateOfBirth = new DateTime(1993, 8, 24),
                SomeSpecialProperty = "asdas"
            };

            Person pNotValid = new Person()
            {
                FirstName = "Jan",
                LastName = "Novák",
                DateOfBirth = new DateTime(1993, 8, 24),
                SomeSpecialProperty = null
            };

            PersonValidator personValidator = new PersonValidator();


            ValidationResult results = personValidator.Validate(pValid);
            if (results.IsValid)
            {
                Console.WriteLine("ok");
            }
            else
            {
                Console.WriteLine(results.Errors);
            }

            ValidationResult results2 = personValidator.Validate(pNotValid);
            if (results2.IsValid)
            {
                Console.WriteLine("ok");
            }
            else
            {
                results2.Errors.ToList().ForEach(i => Debug.WriteLine(i.ToString()));
            }

            // Valid only single property
            ValidationResult results3 = ValidationExtensions.Validate(personValidator, pValid, "SomeSpecialProperty");
            ValidationResult results4 = ValidationExtensions.Validate(personValidator, pNotValid, "SomeSpecialProperty");

        }
    }
}
