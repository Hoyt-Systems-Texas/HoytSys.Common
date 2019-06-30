using System;
using Mrh.Messaging.Common;

namespace Mrh.Messaging.NetMq
{
    public interface IPayloadTypeEncoder<TPayloadType, TBody> where TPayloadType:struct
    {
        bool Encode(MessageEnvelope<TPayloadType, TBody> envelope, Span<byte> position);

        TPayloadType Decode(ReadOnlySpan<byte> value);
    }
}