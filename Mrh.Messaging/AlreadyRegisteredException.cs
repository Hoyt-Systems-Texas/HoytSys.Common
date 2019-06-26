using System;

namespace Mrh.Messaging
{
    public class AlreadyRegisteredException<TPayloadType> : Exception where TPayloadType:struct
    {
        public readonly TPayloadType PayloadType;

        public AlreadyRegisteredException(TPayloadType payloadType) : base($"This payload of {payloadType} has already been registered.")
        {
            this.PayloadType = payloadType;
        }
    }
}