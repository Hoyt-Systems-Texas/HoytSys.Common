using System;

namespace A19.DocumentStore.Common.Message
{
    public class Owner
    {
        public int OwnerType { get; set; }
        
        public Guid OwnerId { get; set; }
    }
}