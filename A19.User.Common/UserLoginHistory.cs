using System;

namespace A19.User.Common
{
    public class UserLoginHistory
    {
        /// <summary>
        ///     The id of the user who is trying to login.
        /// </summary>
        public long? UserId { get; set; }
        
        public string Username { get; set; }
        
        /// <summary>
        ///     The system id.
        /// </summary>
        public int SystemId { get; set; }
        
        /// <summary>
        ///     The date time of the login.
        /// </summary>
        public DateTime LoginOn { get; set; }
        
        /// <summary>
        ///     The ip address the user is trying to login as.
        /// </summary>
        public string IpAddress { get; set; }
    }
}