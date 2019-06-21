using System;

namespace Mrh.Messaging
{
    public interface IOutgoingConnection<TPayloadType, TBody> : IDisposable where TPayloadType:struct
    {

        /// <summary>
        ///     Used to connect to the outgoing connection.
        /// </summary>
        void Connect();
        
        /// <summary>
        ///     Who to send the message to.
        /// </summary>
        /// <param name="message">The message to send to the user.</param>
        void Send(Message<TPayloadType, TBody> message);
    }
}