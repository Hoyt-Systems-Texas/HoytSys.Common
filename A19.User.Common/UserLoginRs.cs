using System;

namespace A19.Security.User
{
    public class UserLoginRs
    {
        public bool Success { get; set; }
        
        public Guid UserGuid { get; set; }
    }
}