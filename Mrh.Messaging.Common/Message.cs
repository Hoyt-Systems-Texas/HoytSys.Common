using System;
using Mrh.Messaging.Common;

namespace Mrh.Messaging
{
    /// <summary>
    ///     Represents an unprocessed message.
    /// </summary>
    /// <typeparam name="TPayloadType">The type for the payload.</typeparam>
    /// <typeparam name="TBody">The type for the body.</typeparam>
    public class Message<TPayloadType, TBody> where TPayloadType : struct
    {
        /// <summary>
        ///     The application generated request id.
        /// </summary>
        public long RequestId { get; set; }

        /// <summary>
        ///     The message identifier which also includes the person sending the message.
        /// </summary>
        public MessageIdentifier MessageIdentifier { get; set; }

        /// <summary>
        ///     Used for an even on who to send the message to.
        /// </summary>
        public Guid? ToConnectionId { get; set; }

        /// <summary>
        ///     The type of message.
        /// </summary>
        public MessageType MessageType { get; set; }

        /// <summary>
        ///     The result of processing the message.
        /// </summary>
        public MessageResultType MessageResultType { get; set; }

        /// <summary>
        ///     The body of the message.
        /// </summary>
        public TBody Body { get; set; }

        /// <summary>
        ///     The type of the payload.
        /// </summary>
        public TPayloadType PayloadType { get; set; }

        /// <summary>
        ///     The id of the user sending the message.  DO NOT store on an untrusted connection.
        /// </summary>
        public Guid UserId { get; set; }
    }

}