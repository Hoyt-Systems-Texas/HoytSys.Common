using System.Collections.Generic;
using HoytSys.Core;
using A19.Messaging.Common;

namespace A19.User.Common
{
    public class UserLoginRq : IValidatable, IRequest
    {
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        /// <summary>
        ///     The id of the system the user is logging into.
        /// </summary>
        public int SystemId { get; set; }
        
        /// <summary>
        ///     The ip address for the user who is trying to login.
        /// </summary>
        public string IpAddress { get; set; }
        
        public List<ValidationError> Validate(List<ValidationError> errors)
        {
            errors
                .Required(nameof(Username), Username)
                .Required(nameof(Password), Password);
            return errors;
        }
    }
}