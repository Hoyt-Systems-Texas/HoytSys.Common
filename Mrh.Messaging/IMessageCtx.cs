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
        
        /// <summary>
        ///     The current state of the message. DO NOT set this value and this implementation needs to be thread safe.
        /// </summary>
        MessageState CurrentState { get; }

        /// <summary>
        ///     Used to change the state and it must be done atomically using the Interlock.CompareAndExchange.
        /// </summary>
        /// <param name="newState">The new state to set.</param>
        /// <param name="oldState">The old state to compare the value.</param>
        /// <returns>true if we where able to set the state.</returns>
        bool TrySetState(MessageState newState, MessageState oldState);
    }
}