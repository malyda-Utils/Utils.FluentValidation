using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.FluentValidation.Entity;

namespace Utils.FluentValidation
{
    class SpecialPropertyValidator : AbstractValidator<SpecialProperty>
    {
        public SpecialPropertyValidator()
        {
            RuleFor(specialProperty => specialProperty.Property)
                .NotEmpty()
                .NotNull()
                .WithMessage("Property is empty or null"); 
        }
    }
}
