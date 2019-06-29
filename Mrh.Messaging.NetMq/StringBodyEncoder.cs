using System;
using System.Text;
using Mrh.Messaging.Common;

namespace Mrh.Messaging.NetMq
{
    public class StringBodyEncoder<TPayloadType> : IEncoder<TPayloadType, string> where TPayloadType : struct
    {

        private const int NUMBER = 4;
        private const int TOTAL = 8;
        private const int TOTAL_BODY = 12;
        private const int MESSAGE_TYPE = 16;
        private const int MESSAGE_RESULT_TYPE = 20;
        private const int CONNECTION_ID = 24;
        private const int CORRELATION_ID = 40;
        private const int USER_ID = 44;
        private const int LENGTH = 60;
        private const int PAYLOAD_TYPE = 64;
        private const int REQUEST_ID = 68;
        private const int TO_CONNECTION = 76;
        private const int BODY = 96;
        private const int GUID_LENGTH = 16;
        private const int INT_LENGTH = 4;
        private const int LONG_LENGTH = 8;
        private const int SHORT_LENGTH = 2;

        private readonly IPayloadTypeEncoder<TPayloadType, string> payloadTypeEncoder;

        public StringBodyEncoder(
            IPayloadTypeEncoder<TPayloadType, string> payloadTypeEncoder)
        {
            this.payloadTypeEncoder = payloadTypeEncoder;
        }
        
        
        public bool Decode(byte[] frame, out MessageEnvelope<TPayloadType, string> message)
        {
            if (frame.Length >= MessageSettings.HEADER_SIZE)
            {
                var number = BitConverter.ToInt32(frame, NUMBER);
                var total = BitConverter.ToInt32(frame, TOTAL);
                var bodyLength = BitConverter.ToInt32(frame, TOTAL_BODY);
                var messageType = (MessageType) BitConverter.ToInt32(frame, MESSAGE_TYPE);
                var messageResult = (MessageResultType) BitConverter.ToInt32(frame, MESSAGE_RESULT_TYPE);
                var connectionId = new Guid(new ReadOnlySpan<byte>(frame, CONNECTION_ID, GUID_LENGTH));
                var correlationId = BitConverter.ToInt32(frame, CORRELATION_ID);
                var userId = new Guid(new ReadOnlySpan<byte>(frame, USER_ID, GUID_LENGTH));
                var length = BitConverter.ToInt32(frame, LENGTH);
                var payloadType = this.payloadTypeEncoder.Decode(this.CreateInt(ref frame, PAYLOAD_TYPE));
                var requestId = BitConverter.ToInt64(frame, REQUEST_ID);
                var toConnectionId = new Guid(new ReadOnlySpan<byte>(frame, TO_CONNECTION, GUID_LENGTH));
                string body = null;
                if (length > 0
                    && frame.Length >= length + MessageSettings.HEADER_SIZE)
                {
                    var bodySpan = new ReadOnlySpan<byte>(
                        frame,
                        BODY,
                        length);
                    body = Encoding.UTF8.GetString(bodySpan);
                }

                message = new MessageEnvelope<TPayloadType, string>()
                {
                    Body = body,
                    Number = number,
                    Total = total,
                    ConnectionId = connectionId,
                    CorrelationId = correlationId,
                    MessageType = messageType,
                    PayloadType = payloadType,
                    UserId = userId,
                    TotalBodyLength = bodyLength,
                    MessageResultType = messageResult,
                    RequestId = requestId,
                    ToConnectionId = toConnectionId
                };
                return true;
            }

            message = null;
            return false;
        }

        /// <summary>
        ///     Used to encode a message to the passed byte buffer.
        /// </summary>
        /// <param name="message">The message to encode.</param>
        /// <param name="buffer">The buffer we are writing the values too.</param>
        public void Encode(MessageEnvelope<TPayloadType, string> message, ref byte[] buffer)
        {
            Array.Clear(buffer, 0, buffer.Length);
            BitConverter.TryWriteBytes(CreateInt(ref buffer, NUMBER), message.Number);
            BitConverter.TryWriteBytes(CreateInt(ref buffer, TOTAL), message.Total);
            BitConverter.TryWriteBytes(CreateInt(ref buffer, TOTAL_BODY), message.TotalBodyLength);
            BitConverter.TryWriteBytes(CreateInt(ref buffer, MESSAGE_TYPE), (int) message.MessageType);
            BitConverter.TryWriteBytes(CreateInt(ref buffer, MESSAGE_RESULT_TYPE), (int) message.MessageResultType);
            message.ConnectionId.TryWriteBytes(CreateGuid(ref buffer, CONNECTION_ID));
            BitConverter.TryWriteBytes(CreateInt(ref buffer, CORRELATION_ID), message.CorrelationId);
            message.UserId.TryWriteBytes(CreateGuid(ref buffer, USER_ID));
            if (message.Body != null)
            {
                BitConverter.TryWriteBytes(CreateInt(ref buffer, LENGTH), message.Body.Length);
                Encoding.UTF8.GetBytes(message.Body, new Span<byte>(buffer, BODY, message.Body.Length));
            }
            BitConverter.TryWriteBytes(CreateLong(ref buffer, REQUEST_ID), message.RequestId);
            this.payloadTypeEncoder.Encode(message, CreateInt(ref buffer, PAYLOAD_TYPE));
        }
        
        private Span<byte> CreateInt(ref byte[] buffer, int start)
        {
            return new Span<byte>(buffer, start, INT_LENGTH);
        }
        
        private Span<byte> CreateLong(ref byte[] buffer, int start)
        {
            return new Span<byte>(buffer, start, LONG_LENGTH);
        }

        private Span<Byte> CreateGuid(ref byte[] buffer, int start)
        {
            return new Span<byte>(buffer, start, GUID_LENGTH);
        }
    }
}