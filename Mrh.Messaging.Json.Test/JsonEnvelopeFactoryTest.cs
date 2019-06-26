using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Mrh.Messaging.Json.Test
{
    [TestFixture]
    public class JsonEnvelopeFactoryTest
    {

        [Test]
        public void OneSplit()
        {
            var envelope = new JsonEnvelopeFactory<PayloadType>(
                100);
            var msg = new Message<PayloadType, string>
            {
                Body = "Hello World!",
                MessageIdentifier = new MessageIdentifier("1", 1),
                MessageType = MessageType.Reply,
                PayloadType = PayloadType.Test,
                RequestId = 0,
                UserId = Guid.NewGuid(),
                MessageResultType = MessageResultType.Success,
                ToConnectionId = "me"
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
                4);
            var msg = new Message<PayloadType, string>
            {
                Body = "Hello World!",
                MessageIdentifier = new MessageIdentifier("1", 1),
                MessageType = MessageType.Reply,
                PayloadType = PayloadType.Test,
                RequestId = 0,
                UserId = Guid.NewGuid(),
                MessageResultType = MessageResultType.Success,
                ToConnectionId = "me"
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
                4);
            var msg = new Message<PayloadType, string>
            {
                Body = "Hello World",
                MessageIdentifier = new MessageIdentifier("1", 1),
                MessageType = MessageType.Reply,
                PayloadType = PayloadType.Test,
                RequestId = 0,
                UserId = Guid.NewGuid(),
                MessageResultType = MessageResultType.Success,
                ToConnectionId = "me"
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