using A19.Messaging.Common;

namespace A19.Messaging
{
    public class MessageResult<TBody>
    {
        public long RequestId { get; set; }
        
        public MessageIdentifier MessageIdentifier { get; set; }
        
        public MessageResultType Result { get; set; }
        
        public TBody Body { get; set; }

    }
}