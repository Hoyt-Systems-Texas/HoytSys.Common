using System;

namespace A19.User.Common
{
    public class UserLoginHistory
    {
        public long UserId { get; set; }
        
        public int SystemId { get; set; }
        
        public DateTime LoginOn { get; set; }
    }
}