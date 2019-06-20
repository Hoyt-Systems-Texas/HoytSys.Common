using System;
using Mrh.Concurrent;

namespace Mrh.Messaging
{
    public struct MessageBuilder<TPayloadType, TBodyType> where TPayloadType:struct
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

        private readonly Action<Message<TPayloadType, TBodyType>> messageCompleteHandler;

        public MessageBuilder(
            short total,
            MessageIdentifier messageIdentifier,
            MessageType messageType,
            TPayloadType payloadType,
            Guid userId,
            IBodyReconstructor<TBodyType> bodyReconstructor,
            Action<Message<TPayloadType, TBodyType>> messageCompleteHandler)
        {
            this.LastTouched = new StopWatchThreadSafe();
            this.Total = total;
            this.BodyReconstructor = bodyReconstructor;
            this.MessageIdentifier = messageIdentifier;
            this.PayloadType = payloadType;
            this.UserId = userId;
            this.MessageType = messageType;
            this.messageCompleteHandler = messageCompleteHandler;
        }

        /// <summary>
        ///     Used to append a body to a frame.
        /// </summary>
        /// <param name="position">The position to add to the body.</param>
        /// <param name="body"></param>
        public void Append(int position, TBodyType body)
        {
            this.BodyReconstructor.Append(position, body);
            if (this.BodyReconstructor.Completed())
            {
                this.messageCompleteHandler(new Message<TPayloadType, TBodyType>()
                {
                    Body = BodyReconstructor.Body,
                    MessageIdentifier = this.MessageIdentifier,
                    MessageType = this.MessageType,
                    PayloadType = this.PayloadType,
                });
            }
        }
    }
}