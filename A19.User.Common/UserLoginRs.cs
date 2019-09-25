using System;
using System.Collections.Generic;
using A19.User.Common;

namespace A19.Security.User
{
    public class UserLoginRs
    {
        /// <summary>
        ///     true if they where able to login successfully.
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        ///     The session key for the user.
        /// </summary>
        public string SessionKey { get; set; }
        
        /// <summary>
        ///     The user's guid id.
        /// </summary>
        public Guid UserGuid { get; set; }
        
        /// <summary>
        ///     The list of roles a user has.
        /// </summary>
        public List<Role> Roles { get; set; }
    }
}