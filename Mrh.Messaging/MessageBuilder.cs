using System;
using Mrh.Concurrent;

namespace Mrh.Messaging
{
    public class MessageBuilder<TPayloadType, TBodyType> where TPayloadType:struct
    {
        /// <summary>
        ///     The last time this message was touched.
        /// </summary>
        public readonly StopWatchThreadSafe LastTouched;

        /// <summary>
        ///     The total number of messages.
        /// </summary>
        public readonly short Total;

        /// <summary>
        ///     The type of the message.
        /// </summary>
        public readonly MessageType MessageType;

        /// <summary>
        ///     The type of the payload.
        /// </summary>
        public readonly TPayloadType PayloadType;

        /// <summary>
        ///     The unique message identifier.
        /// </summary>
        public readonly MessageIdentifier MessageIdentifier;

        /// <summary>
        ///     The unique identifier for the user.
        /// </summary>
        public readonly Guid UserId;

        /// <summary>
        ///     The body fragments needed to reconstruct the message.
        /// </summary>
        public readonly IBodyReconstructor<TBodyType> BodyReconstructor;

        public bool Completed;

        public MessageBuilder(
            short total,
            MessageIdentifier messageIdentifier,
            MessageType messageType,
            TPayloadType payloadType,
            Guid userId,
            IBodyReconstructor<TBodyType> bodyReconstructor)
        {
            this.LastTouched = new StopWatchThreadSafe();
            this.Total = total;
            this.BodyReconstructor = bodyReconstructor;
            this.MessageIdentifier = messageIdentifier;
            this.PayloadType = payloadType;
            this.UserId = userId;
            this.MessageType = messageType;
            this.Completed = false;
        }

        /// <summary>
        ///     Used to append a body to a frame.
        /// </summary>
        /// <param name="position">The position to add to the body.</param>
        /// <param name="body">The body to append to the message.</param>
        public void Append(int position, TBodyType body)
        {
            this.BodyReconstructor.Append(position, body);
            if (this.BodyReconstructor.Completed())
            {
                this.Completed = true;
            }
        }
    }
}