using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.FluentValidation.Entity;

namespace Utils.FluentValidation
{
    /// <summary>
    /// Validator created for Referenced Class to validate its property
    /// </summary>
    class ReferencedClassFromPersonValidator : AbstractValidator<ReferencedClassFromPerson>
    {
        public ReferencedClassFromPersonValidator()
        {
            RuleFor(referencedClass => referencedClass.Property)
                .NotEmpty()
                .NotNull();
        }
    }
}
