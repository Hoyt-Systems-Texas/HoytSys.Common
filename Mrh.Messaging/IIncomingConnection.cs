using System;

namespace Mrh.Messaging
{
    /// <summary>
    ///     Represents an incoming connection.
    /// </summary>
    /// <typeparam name="TPayloadType">The message type.</typeparam>
    /// <typeparam name="TBody">The body type.</typeparam>
    public interface IIncomingConnection<TPayloadType, TBody> : IConnectable where TPayloadType:struct
    {
        /// <summary>
        ///     The handler for the incoming messages.
        /// </summary>
        /// <param name="handler"></param>
        void AddMessageHandler(Action<Message<TPayloadType, TBody>> handler);
    }
}