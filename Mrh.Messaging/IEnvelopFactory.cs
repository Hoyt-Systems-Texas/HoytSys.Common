
using System;
using Mrh.Messaging.Common;

namespace Mrh.Messaging
{
    /// <summary>
    ///     Used to create the envelops we are going to send to the client.
    /// </summary>
    /// <typeparam name="TPayloadType">The type for the payload.</typeparam>
    /// <typeparam name="TBodyType">The type for the body.</typeparam>
    public interface IEnvelopFactory<TPayloadType, TBodyType> where TPayloadType: struct
    {
        /// <summary>
        ///     Used to create the envelops for the client.
        /// </summary>
        /// <param name="message">The messages to create.</param>
        /// <param name="envelopHandler">The handler for processing the envelopment.</param>
        /// <returns>The total number of fragments.</returns>
        int CreateEnvelops(Message<TPayloadType, TBodyType> message, Action<MessageEnvelope<TPayloadType, TBodyType>> envelopHandler);

        /// <summary>
        ///     Used to resend a frame to a user.
        /// </summary>
        /// <param name="message">The message we are sending to the user.</param>
        /// <param name="frameNumber">The number on the frame.</param>
        /// <param name="envelopHandler">The handler to call with the frame.</param>
        void CreateFragment(Message<TPayloadType, TBodyType> message, int frameNumber,
            Action<MessageEnvelope<TPayloadType, TBodyType>> envelopHandler);
    }
}