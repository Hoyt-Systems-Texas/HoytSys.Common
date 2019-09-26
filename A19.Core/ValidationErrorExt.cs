using System.Collections.Generic;

namespace A19.Core
{
    public static class ValidationErrorExt
    {

        /// <summary>
        ///     Validates there is a value for a field.
        /// </summary>
        /// <param name="errors">The list containing the errors.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        /// <param name="prefix">The prefix for the path to the field.</param>
        /// <returns>The list of the errors.</returns>
        public static List<ValidationError> Required(
            this List<ValidationError> errors,
            string fieldName,
            string value,
            string prefix = "")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add(new ValidationError
                {
                    FieldName = fieldName,
                    Description = $"The field {prefix}{fieldName} is required."
                });
            }
            return errors;
        }

        /// <summary>
        ///     Validates the maximum length.
        /// </summary>
        /// <param name="errors">The current list of errors.</param>
        /// <param name="length">The maximum length.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="value">The value for the field.</param>
        /// <param name="prefix">The current prefix.</param>
        /// <returns>The list of errors.</returns>
        public static List<ValidationError> MaxLength(
            this List<ValidationError> errors,
            int length,
            string fieldName,
            string value,
            string prefix = "")
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length > length)
            {
                errors.Add(new ValidationError
                {
                    FieldName = fieldName,
                    Description = $"The field {prefix}{fieldName} length must be less than {length}."
                });
            }

            return errors;
        }

        /// <summary>
        ///     Validates the minimum length.
        /// </summary>
        /// <param name="errors">The current list of errors.</param>
        /// <param name="length">The minimum length of the field.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        /// <param name="prefix">The current prefix for the field.</param>
        /// <returns>The list of errors.</returns>
        public static List<ValidationError> MinLength(
            this List<ValidationError> errors,
            int length,
            string fieldName,
            string value,
            string prefix = "")
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length < length)
            {
                errors.Add(new ValidationError
                {
                    FieldName = fieldName,
                    Description = $"The field {prefix}{fieldName} length must be greather than {length}."
                });
            }
            return errors;
        }

        public static List<ValidationError> MinValue(
            this List<ValidationError> errors,
            int minValue,
            string fieldName,
            int? value,
            string prefix = "")
        {
            if (value.HasValue)
            {
                return errors.MinValue(
                    minValue,
                    fieldName,
                    value.Value,
                    prefix);
            }

            return errors;
        }
        
        public static List<ValidationError> MinValue(
            this List<ValidationError> errors,
            int minValue,
            string fieldName,
            int value,
            string prefix = "")
        {
            if (value < minValue)
            {
                errors.Add(new ValidationError
                {
                    FieldName = fieldName,
                    Description = $"The field {prefix}{fieldName} minimum value is {minValue}."
                });
            }
            return errors;
        }
        
        public static List<ValidationError> MaxValue(
            this List<ValidationError> errors,
            int maxValue,
            string fieldName,
            int value,
            string prefix = "")
        {
            if (value > maxValue)
            {
                errors.Add(new ValidationError
                {
                    FieldName = fieldName,
                    Description = $"The field {prefix}{fieldName} maximum value is {maxValue}."
                });
            }
            return errors;
        }
    }
}