using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Utils.FluentValidation
{
    class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(person => person.FirstName)
                .NotEmpty()
                .NotNull()
                .WithMessage("First name is empty");

            RuleFor(person => person.LastName)
                .NotEqual(person => person.FirstName)
                .WithMessage("First name and last name could not be same");

            RuleFor(person => person.DateOfBirth)
                .NotEqual(DateTime.Now)
                .WithMessage("Inserted date could not be today");

            RuleFor(person => person.SomeSpecialProperty)
                .NotEmpty()
                .Must(SpecialPropertyValidator)
                .WithMessage("Error message");
        }

        private bool SpecialPropertyValidator(object SpecialProperty)
        {
            // if some validation pass return true
            return true;
        }


    }
}
