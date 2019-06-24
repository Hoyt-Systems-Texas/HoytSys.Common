using System.Collections.Concurrent;

namespace Mrh.Messaging
{
    public class IncomingMessageBuilder<TPayloadType, TBody> : IIncomingMessageBuilder<TPayloadType, TBody>
        where TPayloadType : struct
    {
        private readonly ConcurrentDictionary<MessageIdentifier, MessageBuilder<TPayloadType, TBody>> pendingMessages =
            new ConcurrentDictionary<MessageIdentifier, MessageBuilder<TPayloadType, TBody>>(10, 1000);

        private readonly IOutgoingConnection<TPayloadType, TBody> outgoingConnection;

        public IncomingMessageBuilder(
            IOutgoingConnection<TPayloadType, TBody> outgoingConnection)
        {
            this.outgoingConnection = outgoingConnection;
        }

        public bool Add(
            MessageEnvelope<TPayloadType, TBody> envelope,
            out Message<TPayloadType, TBody> message)
        {
            var identifier = new MessageIdentifier(
                envelope.ConnectionId,
                envelope.CorrelationId);
            if (envelope.Total <= 0)
            {
                message = new Message<TPayloadType, TBody>()
                {
                    Body = envelope.Body,
                    MessageIdentifier = identifier,
                    MessageType = envelope.MessageType,
                    PayloadType = envelope.PayloadType,
                };
                return true;
            }

            MessageBuilder<TPayloadType, TBody> messageBuilder;
            if (this.pendingMessages.TryGetValue(
                identifier,
                out messageBuilder))
            {
                messageBuilder.Append(envelope.Number, envelope.Body);
                if (messageBuilder.Completed)
                {
                }
            }
            throw new System.NotImplementedException();
        }
    }
}