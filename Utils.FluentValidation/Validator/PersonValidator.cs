using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using FluentValidation.Results;
using Utils.FluentValidation.Entity;

namespace Utils.FluentValidation.Validator
{
    /// <summary>
    /// Person validator declares set of rules for person to be valid and error messages if not
    /// </summary>
    class PersonValidator : AbstractValidator<Person>
    {
        /// <summary>
        /// Declares rules for every single property of Person
        /// </summary>
        public PersonValidator()
        {
            RuleFor(person => person.FirstName)
                .NotEmpty()
                .NotNull();

            RuleFor(person => person.LastName)
                .NotEqual(person => person.FirstName)
                .WithMessage("First name and last name could not be same");

            RuleFor(person => person.DateOfBirth)
                .NotEqual(DateTime.Now)
                .WithMessage("Inserted date could not be today");
            
            // Just validate referenced object
            RuleFor(person => person.ReferenceToAnotherClass)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                    .WithMessage("ReferenceToAnotherClass is null from SimplePropertyValidator")
               .Must(SimplePropertyValidator)
                    .WithMessage("ReferenceToAnotherClass is not valid by SimplePropertyValidator ");
            
            // Validate referenced object and print errors
            RuleFor(person => person.ReferenceToAnotherClass)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                    .WithMessage("ReferenceToAnotherClass is null from ExtendedPropertyValidator")
                .Custom((person, context) =>
                {
                    ValidationResult result = ExtendedPropertyValidator(person);   
                    if (!result.IsValid)
                    {
                        context.AddFailure(
                            string.Join(",", result.Errors)
                            +" by ExtendedPropertyValidator");
                    } 
                });
        }

        /// <summary>
        /// Validate property which is reference to another class which needs to be valid too
        /// </summary>
        /// <param name="SpecialProperty">Instance of referenced class</param>
        /// <returns>ValidationResult</returns>
        private ValidationResult ExtendedPropertyValidator(SpecialProperty SpecialProperty)
        {
            // Validator for given class
            SpecialPropertyValidator specialPropertyValidator = new SpecialPropertyValidator();
            return specialPropertyValidator.Validate(SpecialProperty);
        }


        /// <summary>
        /// Validate property which is reference to another class which needs to be valid too
        /// </summary>
        /// <param name="SpecialProperty">Instance of referenced class</param>
        /// <returns>True if referenced class is valid by its own validator</returns>
        private bool SimplePropertyValidator(SpecialProperty SpecialProperty)
        {
            // Validator for given class
            SpecialPropertyValidator specialPropertyValidator = new SpecialPropertyValidator();
            return specialPropertyValidator.Validate(SpecialProperty).IsValid;
        }

    }
}
