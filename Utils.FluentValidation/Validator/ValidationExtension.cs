using System;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;

namespace Utils.FluentValidation
{
    static class ValidationExtensions
    {
        /// <summary>
        /// Used for validation of single property with existing validator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validator">Validator for class</param>
        /// <param name="instance">Instance of class to be validated</param>
        /// <param name="properties">Name of property to validate</param>
        /// <returns></returns>
        public static ValidationResult Validate<T>(this IValidator validator, T instance, params string[] properties)
        {
            var context = new ValidationContext<T>(instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(properties));
            return validator.Validate(context);
        }

    }
}
