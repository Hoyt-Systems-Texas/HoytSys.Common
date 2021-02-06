using System;
using System.Collections.Generic;
using HoytSys.Core;

namespace A19.User.Common
{
    public class User : IValidatable
    {
        public long UserId { get; set; }

        public Guid UserGuid { get; set; }
        
        public UserAuthenticateType UserAuthenticateType { get; set; }
        
        public string TempPassword { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        /// <summary>
        ///     The id of the roles for the user.
        /// </summary>
        public List<int> Roles { get; set; }

        public List<ValidationError> Validate(List<ValidationError> errors)
        {
            errors.Required(
                    nameof(Username),
                    Username)
                .MaxLength(
                    100,
                    nameof(Username),
                    Username)
                .MaxLength(
                    100,
                    nameof(LastName),
                    LastName)
                .Required(
                    nameof(LastName),
                    LastName)
                .MaxLength(
                    100,
                    nameof(FirstName),
                    FirstName);
            return errors;
        }
    }
}