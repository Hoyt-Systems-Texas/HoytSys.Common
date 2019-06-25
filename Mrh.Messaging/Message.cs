using System;

namespace Mrh.Messaging
{
    /// <summary>
    ///     Represents an unprocessed message.
    /// </summary>
    /// <typeparam name="TPayloadType">The type for the payload.</typeparam>
    /// <typeparam name="TBody">The type for the body.</typeparam>
    public class Message<TPayloadType, TBody> where TPayloadType: struct
    {
        
        /// <summary>
        ///     The application generated request id.
        /// </summary>
        public long RequestId { get; set; }
        
        /// <summary>
        ///     The message identifier which also includes the person sending the message.
        /// </summary>
        public MessageIdentifier MessageIdentifier { get; set; }
        
        /// <summary>
        ///     Used for an even on who to send the message to.
        /// </summary>
        public string ToConnectionId { get; set; }
        
        public MessageType MessageType { get; set; }
        
        /// <summary>
        ///     The result of processing the message.
        /// </summary>
        public MessageResultType MessageResultType { get; set; }
        
        public TBody Body { get; set; }
        
        public TPayloadType PayloadType { get; set; }
        
        public Guid UserId { get; set; }
    }
}