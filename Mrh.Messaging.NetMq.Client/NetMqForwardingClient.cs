using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Mrh.Concurrent;
using Mrh.Messaging.Client;
using Mrh.Messaging.Common;
using Mrh.Monad;
using NLog;

namespace Mrh.Messaging.NetMq.Client
{
    
    /// <summary>
    ///      A basic client used for forwarding messages sent using zero mq.
    /// </summary>
    /// <typeparam name="TPayloadType"></typeparam>
    /// <typeparam name="TBody"></typeparam>
    public class NetMqForwardingClient<TPayloadType, TBody> : IForwardingClient<TPayloadType, TBody>,
        IIncomingMessageHandler<TPayloadType, TBody> where TPayloadType : struct
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly Guid connectionId;
        private int correlationId;
        private readonly IBodyEncoder<TBody> encoder;
        private readonly IOutgoingConnection<TPayloadType, TBody> outgoingConnection;

        private readonly ConcurrentDictionary<MessageIdentifier, PendingRequestNode> pendingRequestNodes =
            new ConcurrentDictionary<MessageIdentifier, PendingRequestNode>(4, 1000);

        public NetMqForwardingClient(
            IBodyEncoder<TBody> encoder,
            IOutgoingConnection<TPayloadType, TBody> outgoingConnection,
            IConnectionIdGenerator connectionIdGenerator)
        {
            this.connectionId = connectionIdGenerator.Generate();
            this.encoder = encoder;
            this.outgoingConnection = outgoingConnection;
        }

        public void Connect()
        {
        }

        public void Disconnect()
        {
        }

        public Task<Message<TPayloadType, TBody>> Send<T>(TPayloadType payloadType, T body, Guid userId)
        {
            var identifier = new MessageIdentifier(
                this.connectionId,
                Interlocked.Increment(ref this.correlationId));
            var msg = new Message<TPayloadType, TBody>
            {
                Body = encoder.Encode(body),
                MessageIdentifier = identifier,
                MessageType = MessageType.Request,
                PayloadType = payloadType,
                UserId = userId,
            };
            var pendingNode = new PendingRequestNode();
            this.pendingRequestNodes[identifier] = pendingNode;
            this.outgoingConnection.Send(msg);
            return pendingNode.task.Task;
        }

        public bool Handle(Message<TPayloadType, TBody> message)
        {
            switch (message.MessageType)
            {
                case MessageType.Ping:
                    break;

                case MessageType.Pong:
                    break;

                case MessageType.Reply:
                    if (this.pendingRequestNodes.TryGetValue(
                        message.MessageIdentifier,
                        out var node))
                    {
                        node.handle(message);
                    }

                    return true;
            }

            return false;
        }

        public void Start()
        {
            this.Connect();
        }

        public void Stop()
        {
            this.Disconnect();
        }

        private class PendingRequestNode
        {
            public readonly TaskCompletionSource<Message<TPayloadType, TBody>> task =
                new TaskCompletionSource<Message<TPayloadType, TBody>>(TaskCreationOptions
                    .RunContinuationsAsynchronously);

            private readonly StopWatchThreadSafe stopWatchThreadSafe = new StopWatchThreadSafe();

            public void handle(
                Message<TPayloadType, TBody> msg)
            {
                this.task.SetResult(msg);
            }
        }
    }
}