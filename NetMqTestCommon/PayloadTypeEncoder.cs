using System;
using A19.Messaging.Common;
using A19.Messaging.NetMq;
using Mrh.Messaging;

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