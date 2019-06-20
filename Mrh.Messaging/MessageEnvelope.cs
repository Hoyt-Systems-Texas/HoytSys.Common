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
        public MessageType MessageType { get; set; }
        public TPayloadType PayloadType { get; set; }
        public MessageResult MessageResult { get; set; }
        public string ConnectionId { get; set; }
        public int CorrelationId { get; set; }
        public Guid UserId { get; set; }
        public TBodyType Body { get; set; }
        
    }
}