using System;
using A19.Messaging.Common;

namespace A19.Messaging.NetMq
{
    public interface IPayloadTypeEncoder<TPayloadType, TBody> where TPayloadType:struct
    {
        bool Encode(MessageEnvelope<TPayloadType, TBody> envelope, Span<byte> position);

        TPayloadType Decode(ReadOnlySpan<byte> value);
    }
}