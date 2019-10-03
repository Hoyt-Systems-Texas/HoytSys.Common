using System;

namespace A19.User.Common
{
    public class UserSystem
    {
        /// <summary>
        ///     The id of the system the user has login to.
        /// </summary>
        public int SystemId { get; set; }
        
        /// <summary>
        ///     The current session key for the user on that system.
        /// </summary>
        public string SessionKey { get; set; }
        
        /// <summary>
        ///     The last login for the user.
        /// </summary>
        public DateTime LoginOn { get; set; }
        
        /// <summary>
        ///     The last time the user has been seen.
        /// </summary>
        public DateTime LastScene { get; set; }
        
        /// <summary>
        ///     The user session timeout.
        /// </summary>
        public DateTime Timeout { get; set; }


        public bool IsActive
        {
            get { return Timeout > DateTime.Now;  }
        }
    }
}