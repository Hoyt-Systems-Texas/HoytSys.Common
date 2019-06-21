using System;

namespace Mrh.Messaging
{
    /// <summary>
    ///     Represents an incoming connection.
    /// </summary>
    /// <typeparam name="TPayloadType">The message type.</typeparam>
    /// <typeparam name="TBody">The body type.</typeparam>
    public interface IIncomingConnection<TPayloadType, TBody> : IDisposable
    {
        /// <summary>
        ///     Used to connect to the service.
        /// </summary>
        void Connect();
    }
}