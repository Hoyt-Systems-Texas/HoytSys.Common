using System;

namespace Mrh.Messaging
{
    public class UnknownHandlerException<TPayloadType> : Exception where TPayloadType:struct
    {

        public readonly TPayloadType PayloadType;

        public UnknownHandlerException(TPayloadType payloadType) : base($"Unable to find handler for {payloadType}.")
        {
            this.PayloadType = payloadType;
        }
    }
}