using Mrh.Concurrent;

namespace Mrh.Messaging
{
    public class IncomingMessageProcessor<TPayloadType, TBody> where TPayloadType : struct
    {
        private MpmcRingBuffer<MessageEnvelope<TPayloadType, TBody>> buffer =
            new MpmcRingBuffer<MessageEnvelope<TPayloadType, TBody>>(0x1000);

        public IncomingMessageProcessor()
        {
        }
    }
}