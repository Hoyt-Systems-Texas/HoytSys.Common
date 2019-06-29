using System;
using Mrh.Messaging;
using Mrh.Messaging.NetMq;

namespace ServiceApplicationTester
{
    public class PayloadTypeEncoder : IPayloadTypeEncoder<PayloadType, string>
    {
        public bool Encode(MessageEnvelope<PayloadType, string> envelope, Span<byte> position)
        {
            return BitConverter.TryWriteBytes(position, (int) envelope.PayloadType);
        }

        public PayloadType Decode(ReadOnlySpan<byte> value)
        {
            return (PayloadType) BitConverter.ToInt32(value);
        }
    }
}