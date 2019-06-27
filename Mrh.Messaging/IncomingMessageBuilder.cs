using System.Collections.Concurrent;

namespace Mrh.Messaging
{
    public class IncomingMessageBuilder<TPayloadType, TBody> : IIncomingMessageBuilder<TPayloadType, TBody>
        where TPayloadType : struct
    {
        private readonly ConcurrentDictionary<MessageIdentifier, MessageBuilder<TPayloadType, TBody>> pendingMessages =
            new ConcurrentDictionary<MessageIdentifier, MessageBuilder<TPayloadType, TBody>>(10, 1000);
        private readonly IBodyReconstructorFactory<TBody> bodyReconstructorFactory;

        public IncomingMessageBuilder(
            IBodyReconstructorFactory<TBody> bodyReconstructorFactory)
        {
            this.bodyReconstructorFactory = bodyReconstructorFactory;
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
                    message = new Message<TPayloadType, TBody>
                    {
                        Body = messageBuilder.BodyReconstructor.Body,
                        MessageIdentifier = messageBuilder.MessageIdentifier,
                        MessageType = messageBuilder.MessageType,
                        PayloadType = messageBuilder.PayloadType,
                        UserId = messageBuilder.UserId
                    };
                    this.pendingMessages.TryRemove(identifier, out messageBuilder);
                    return true;
                }
                message = null;
                return false;
            }
            else
            {
                messageBuilder = new MessageBuilder<TPayloadType, TBody>(
                    envelope.Total,
                    identifier,
                    envelope.MessageType,
                    envelope.PayloadType,
                    envelope.UserId,
                    this.bodyReconstructorFactory.Create(envelope.Total));
                messageBuilder.Append(envelope.Number, envelope.Body);
                this.pendingMessages[identifier] = messageBuilder;
                message = null;
                return false;
            }
        }
    }
}