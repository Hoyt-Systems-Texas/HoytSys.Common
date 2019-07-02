using System;

namespace NetMqTestCommon
{
    public class User
    {
        public long UserAppId { get; set; }
        
        public Guid UserGuid { get; set; }
        
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public DateTime LastModified { get; set; }
    }
}