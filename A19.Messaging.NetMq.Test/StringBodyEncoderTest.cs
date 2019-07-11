using System;
using A19.Messaging.Common;
using NUnit.Framework;

namespace A19.Messaging.NetMq.Test
{
    [TestFixture]
    public class StringBodyEncoderTest
    {
        private readonly StringBodyEncoder<TestEnum> encoder = new StringBodyEncoder<TestEnum>(
            new TestEnumEncoder());

        private enum TestEnum
        {
            Action = 10
        }

        [Test]
        public void EncodeDecodeBodyTest()
        {
            var byteArray = new byte[4096];
            var envelope = new MessageEnvelope<TestEnum, string>
            {
                Body = "Hello",
                Number = 38,
                Total = 1,
                ConnectionId = Guid.NewGuid(),
                CorrelationId = 2,
                MessageType = MessageType.Reply,
                PayloadType = TestEnum.Action,
                RequestId = 123,
                UserId = Guid.NewGuid(),
                MessageResultType = MessageResultType.Error,
                TotalBodyLength = 5
            };
            this.encoder.Encode(envelope, ref byteArray);
            var resultB = this.encoder.Decode(byteArray, out MessageEnvelope<TestEnum, string> result);
            Assert.IsTrue(resultB);
            Assert.AreEqual(envelope.Body, result.Body);
            Assert.AreEqual(envelope.Number, result.Number);
            Assert.AreEqual(envelope.Total, result.Total);
            Assert.AreEqual(envelope.ConnectionId, result.ConnectionId);
            Assert.AreEqual(envelope.CorrelationId, result.CorrelationId);
            Assert.AreEqual(envelope.MessageType, result.MessageType);
            Assert.AreEqual(envelope.PayloadType, result.PayloadType);
            Assert.AreEqual(envelope.RequestId, result.RequestId);
            Assert.AreEqual(envelope.UserId, result.UserId);
            Assert.AreEqual(envelope.MessageResultType, result.MessageResultType);
            Assert.AreEqual(envelope.TotalBodyLength, result.TotalBodyLength);
        }

        [Test]
        public void EmptyBodyTest()
        {
            var byteArray = new byte[4096];
            var envelope = new MessageEnvelope<TestEnum, string>
            {
                Number = 38,
                Total = 1,
                ConnectionId = Guid.NewGuid(),
                CorrelationId = 2,
                MessageType = MessageType.Reply,
                PayloadType = TestEnum.Action,
                RequestId = 123,
                UserId = Guid.NewGuid(),
                MessageResultType = MessageResultType.Error,
                TotalBodyLength = 5
            };
            this.encoder.Encode(envelope, ref byteArray);
            var resultB = this.encoder.Decode(byteArray, out MessageEnvelope<TestEnum, string> result);
            Assert.IsTrue(resultB);
            Assert.AreEqual(envelope.Body, result.Body);
            Assert.AreEqual(envelope.Number, result.Number);
            Assert.AreEqual(envelope.Total, result.Total);
            Assert.AreEqual(envelope.ConnectionId, result.ConnectionId);
            Assert.AreEqual(envelope.CorrelationId, result.CorrelationId);
            Assert.AreEqual(envelope.MessageType, result.MessageType);
            Assert.AreEqual(envelope.PayloadType, result.PayloadType);
            Assert.AreEqual(envelope.RequestId, result.RequestId);
            Assert.AreEqual(envelope.UserId, result.UserId);
            Assert.AreEqual(envelope.MessageResultType, result.MessageResultType);
            Assert.AreEqual(envelope.TotalBodyLength, result.TotalBodyLength);
        }

        private class TestEnumEncoder : IPayloadTypeEncoder<TestEnum, string>
        {
            public bool Encode(MessageEnvelope<TestEnum, string> envelope, Span<byte> position)
            {
                return BitConverter.TryWriteBytes(position, (int) envelope.PayloadType);
            }

            public TestEnum Decode(ReadOnlySpan<byte> value)
            {
                return (TestEnum) BitConverter.ToInt32(value);
            }
        }
    }
}