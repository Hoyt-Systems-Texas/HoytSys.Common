using System;

namespace A19.User.Common
{
    public class UserFailedLoginHistory
    {
        public long UserFailedLoginHistoryId { get; set; }
        
        public long? UserId { get; set; }
        
        public string Username { get; set; }
        
        public int SystemId { get; set; }
        
        public string IpAddress { get; set; }
        
        public DateTime FailedDate { get; set; }
    }
}