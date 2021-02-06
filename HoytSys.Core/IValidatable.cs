using System.Collections.Generic;

namespace HoytSys.Core
{
    public interface IValidatable
    {
        /// <summary>
        ///     The function to call to validate the object.
        /// </summary>
        /// <param name="errors">The list of errors.</param>
        /// <returns>The validation errors.</returns>
        List<ValidationError> Validate(List<ValidationError> errors);
    }
}