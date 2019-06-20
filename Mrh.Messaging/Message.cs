namespace Mrh.Messaging
{
    /// <summary>
    ///     Represents an unprocessed message.
    /// </summary>
    /// <typeparam name="TPayloadType">The type for the payload.</typeparam>
    /// <typeparam name="TBody">The type for the body.</typeparam>
    public class Message<TPayloadType, TBody> where TPayloadType: struct
    {
        
        public long RequestId { get; set; }
        
        public MessageIdentifier MessageIdentifier { get; set; }
        
        public MessageType MessageType { get; set; }
        
        public MessageResult MessageResult { get; set; }
        
        public TBody Body { get; set; }
        
        public TPayloadType PayloadType { get; set; }
    }
}