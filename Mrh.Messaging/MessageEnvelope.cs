using System;

namespace Mrh.Messaging
{
    public class MessageEnvelope<TPayloadType, TBodyType> where TPayloadType:struct
    {
        /// <summary>
        ///     The number of the envelopment.
        /// </summary>
        public short Number { get; set; }
        /// <summary>
        ///     The total number of pieces.
        /// </summary>
        public short Total { get; set; }
        /// <summary>
        ///     The total length of the body.
        /// </summary>
        public short TotalBodyLength { get; set; }
        /// <summary>
        ///     The message type.
        /// </summary>
        public MessageType MessageType { get; set; }
        /// <summary>
        ///     The payload type that is going to handle the message.
        /// </summary>
        public TPayloadType PayloadType { get; set; }
        /// <summary>
        ///     The result of the message.
        /// </summary>
        public MessageResultType MessageResultType { get; set; }
        /// <summary>
        ///     The id of the connection of who sent the message.
        /// </summary>
        public string ConnectionId { get; set; }
        /// <summary>
        ///     The correlation id of the message.
        /// </summary>
        public int CorrelationId { get; set; }
        /// <summary>
        ///     The id of the user.
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        ///     The body of the message.
        /// </summary>
        public TBodyType Body { get; set; }
        
    }
}