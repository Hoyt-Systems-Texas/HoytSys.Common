using System;

namespace A19.User.Common
{
    public class SystemSessionInfo
    {
        
        public int SystemId { get; set; }
            
        public string SessionId { get; set; }
        
        public DateTime ExpiresOn { get; set; }
    }
}