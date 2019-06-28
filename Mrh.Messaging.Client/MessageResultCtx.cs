using System;

namespace Mrh.Messaging.Client
{
    public class MessageResultCtx<TPayloadType, TBody>
    {
        public long RequestId { get; set; }
        
        public Guid ConnectionId { get; set; }
        
        public int CorrelationId { get; set; }
        
    }
}