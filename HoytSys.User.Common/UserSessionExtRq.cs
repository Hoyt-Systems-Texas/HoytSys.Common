using System;

namespace A19.User.Common
{
    public class UserSessionExtRq
    {
        public int SystemId { get; set; }
        
        public Guid UserGuid { get; set; }
        
        public string SessionKey { get; set; }
    }
}