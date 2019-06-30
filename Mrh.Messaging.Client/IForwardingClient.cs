using System;
using Mrh.Core;
using Mrh.Messaging.Common;

namespace Mrh.Messaging.Client
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