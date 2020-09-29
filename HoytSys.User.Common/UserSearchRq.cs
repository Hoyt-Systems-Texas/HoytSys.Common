using System.Collections.Generic;
using A19.Core;

namespace A19.User.Common
{
    public class UserSearchRq : IValidatable
    {
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Start { get; set; }

        public int Limit { get; set; }

        public List<ValidationError> Validate(List<ValidationError> errors)
        {
            errors
                .MinValue(0, nameof(Start), Start)
                .MinValue(0, nameof(Limit), Limit)
                .MaxValue(250, nameof(Limit), Limit);
            return errors;
        }
    }
}