using A19.Messaging.Common;

namespace A19.Messaging
{
    /// <summary>
    ///     Used to handle incoming messages.  Implementation are also responsible for request missing fragments.
    /// </summary>
    /// <typeparam name="TPayloadType">The type for the payload.</typeparam>
    /// <typeparam name="TBody">The body type.</typeparam>
    public interface IIncomingMessageBuilder<TPayloadType, TBody> where TPayloadType:struct
    {

        /// <summary>
        ///     Used to add an incoming message to process.  This call is expected to not be thread safe.
        /// </summary>
        /// <param name="envelope">The envelop to process.</param>
        /// <param name="message">The completed message.</param>
        /// <returns>true - if the message has been completed.</returns>
        bool Add(MessageEnvelope<TPayloadType, TBody> envelope, out Message<TPayloadType, TBody> message);
    }
}