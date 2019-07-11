using System;
using A19.Core;
using A19.Messaging.Common;

namespace A19.Messaging.Client
{
    public interface IForwardingClient<TPayloadType, TBody> : IStartable, IStoppable where TPayloadType: struct
    {
        /// <summary>
        ///     Used to send an envelope to the backend.
        /// </summary>
        /// <param name="messageEnvelope">The envelope to send.</param>
        void Send(MessageEnvelope<TPayloadType, TBody> messageEnvelope);

        /// <summary>
        ///     The observable getting the events that need to be forward.
        /// </summary>
        IObservable<MessageEnvelope<TPayloadType, TBody>> Receive { get; }
    }
}