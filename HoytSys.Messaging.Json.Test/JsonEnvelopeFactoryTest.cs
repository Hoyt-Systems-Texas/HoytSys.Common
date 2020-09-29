using System;
using System.Collections.Generic;
using A19.Messaging.Common;
using Mrh.Messaging.Json;
using NUnit.Framework;

namespace A19.Messaging.Json.Test
{
    [TestFixture]
    public class JsonEnvelopeFactoryTest
    {

        public class MessagingSetting : IMessageSetting
        {
            public short ServerId { get; set; }
            public int ShiftNumber { get; set;  }
            public int MaxFrameSize { get; set;  }
        }

        [Test]
        public void OneSplit()
        {
            var envelope = new JsonEnvelopeFactory<PayloadType>(
                new MessagingSetting
                {
                    MaxFrameSize = 100
                });
            var msg = new Message<PayloadType, string>
            {
                Body = "Hello World!",
                MessageIdentifier = new MessageIdentifier(Guid.NewGuid(), 1),
                MessageType = MessageType.Reply,
                PayloadType = PayloadType.Test,
                RequestId = 0,
                UserId = Guid.NewGuid(),
                MessageResultType = MessageResultType.Success,
                ToConnectionId = Guid.NewGuid()
            };

            var list = new List<MessageEnvelope<PayloadType, string>>(1);
            envelope.CreateEnvelops(
                msg,
                list.Add);
            Assert.IsNotEmpty(list);
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void ThreeSplit()
        {
            var envelope = new JsonEnvelopeFactory<PayloadType>(
                new MessagingSetting
                {
                    MaxFrameSize = 4
                });
            var msg = new Message<PayloadType, string>
            {
                Body = "Hello World!",
                MessageIdentifier = new MessageIdentifier(Guid.NewGuid(), 1),
                MessageType = MessageType.Reply,
                PayloadType = PayloadType.Test,
                RequestId = 0,
                UserId = Guid.NewGuid(),
                MessageResultType = MessageResultType.Success,
                ToConnectionId = Guid.NewGuid()
            };
            var list = new List<MessageEnvelope<PayloadType, string>>(1);
            envelope.CreateEnvelops(
                msg,
                list.Add);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("Hell", list[0].Body);
            Assert.AreEqual("rld!", list[2].Body);
        }

        [Test]
        public void ThreeSplitNotEven()
        {
            var envelope = new JsonEnvelopeFactory<PayloadType>(
                new MessagingSetting
                {
                    MaxFrameSize = 4
                });
            var msg = new Message<PayloadType, string>
            {
                Body = "Hello World",
                MessageIdentifier = new MessageIdentifier(Guid.NewGuid(), 1),
                MessageType = MessageType.Reply,
                PayloadType = PayloadType.Test,
                RequestId = 0,
                UserId = Guid.NewGuid(),
                MessageResultType = MessageResultType.Success,
                ToConnectionId = Guid.NewGuid()
            };
            var list = new List<MessageEnvelope<PayloadType, string>>(1);
            envelope.CreateEnvelops(
                msg,
                list.Add);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("Hell", list[0].Body);
            Assert.AreEqual("rld", list[2].Body);
        }

        private enum PayloadType
        {
            Test = 0
        }
    }
}