using System;

namespace Mrh.Messaging
{
    public interface IOutgoingConnection<TPayloadType, TBody> : IConnectable where TPayloadType:struct
    {
        
        /// <summary>
        ///     Who to send the message to.
        /// </summary>
        /// <param name="message">The message to send to the user.</param>
        void Send(Message<TPayloadType, TBody> message);
    }
}