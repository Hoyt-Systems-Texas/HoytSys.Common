using System;
using A19.Messaging.Common;

namespace A19.User.Common
{
    public class DestroySessionRq : IRequest
    {
        
        public int SystemId { get; set; }
        
        public string SessionId { get; set; }
        
        public Guid UserGuid { get; set; }
    }
}