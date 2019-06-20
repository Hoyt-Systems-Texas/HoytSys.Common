using System;

namespace Mrh.Messaging
{
    public interface IMessageCtx<TPayloadType, TBody>
    {
        /// <summary>
        ///     The unique id of the request.
        /// </summary>
        long RequestId { get; set; }
        
        /// <summary>
        ///     The id of the user.
        /// </summary>
        Guid UserId { get; set; }
        
        /// <summary>
        ///     The unique identifier of the user.
        /// </summary>
        MessageIdentifier MessageIdentifier { get; set; }
        
        /// <summary>
        ///     Is only valid on a reply message or an event message.
        /// </summary>
        MessageResult MessageResult { get; set; }
        
        /// <summary>
        ///     The type of the message.
        /// </summary>
        MessageType MessageType { get; set; }
        
        /// <summary>
        ///     The type for the payload.
        /// </summary>
        TPayloadType PayloadType { get; set; }
        
        /// <summary>
        ///     The body of the message.
        /// </summary>
        TBody Body { get; set; }
    }
}