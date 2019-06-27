namespace Mrh.Messaging.NetMq
{
    public interface IEncoder<TPayloadType, TBody> where TPayloadType:struct
    {

        /// <summary>
        ///     Used to decode a message.
        /// </summary>
        /// <param name="frame">The frame to decode.</param>
        /// <returns>The message envelope that was decoded.</returns>
         bool Decode(byte[] frame, out MessageEnvelope<TPayloadType, TBody> envelope);

        /// <summary>
        ///     Encodes a message.
        /// </summary>
        /// <param name="message"></param>
        void Encode(MessageEnvelope<TPayloadType, TBody> message, ref byte[] buffer);
    }
}