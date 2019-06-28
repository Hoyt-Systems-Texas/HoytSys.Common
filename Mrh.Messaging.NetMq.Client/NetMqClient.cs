using System;
using System.Collections.Concurrent;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Mrh.Concurrent;
using Mrh.Messaging.Client;
using Mrh.Messaging.Common;
using Mrh.Monad;
using NLog;

namespace Mrh.Messaging.NetMq.Client
{
    public class NetMqClient<TPayloadType, TBody> : IClient<TPayloadType, TBody>,
        IIncomingMessageHandler<TPayloadType, TBody> where TPayloadType : struct
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly Guid connectionId;
        private int correlationId = 0;
        private readonly IBodyEncoder<TBody> encoder;
        private readonly IMessageResultMonad<TPayloadType, TBody> messageResultMonad;
        private readonly IOutgoingConnection<TPayloadType, TBody> outgoingConnection;

        private readonly ConcurrentDictionary<TPayloadType, ISubjectNode> events =
            new ConcurrentDictionary<TPayloadType, ISubjectNode>(4, 100);
        private readonly ConcurrentDictionary<MessageIdentifier, IPendingRequestNode> pendingRequestNodes = new ConcurrentDictionary<MessageIdentifier, IPendingRequestNode>(4, 100);

        public NetMqClient(
            IBodyEncoder<TBody> encoder,
            IMessageResultMonad<TPayloadType, TBody> messageResultMonad,
            IOutgoingConnection<TPayloadType, TBody> outgoingConnection,
            IConnectionIdGenerator connectionIdGenerator)
        {
            this.encoder = encoder;
            this.messageResultMonad = messageResultMonad;
            this.outgoingConnection = outgoingConnection;
            this.connectionId = connectionIdGenerator.Generate();
        }

        public void Connect()
        {
        }

        public void Disconnect()
        {
        }

        public IObservable<T> Subscribe<T>(TPayloadType eventType)
        {
            if (!this.events.ContainsKey(eventType))
            {
                this.events.TryAdd(eventType, new SubjectNode<T>());
            }

            if (this.events.TryGetValue(eventType, out var node))
            {
                if (node is SubjectNode<T> typeNode)
                {
                    return typeNode.subject;
                }
                else
                {
                    throw new SubscriptionTypeMismatchedException(
                        $"The type is already defined on the subscription {node}");
                }
            }
            else
            {
                throw new Exception($"Error occurred while trying to add the subject.");
            }
        }

        public Task<IResultMonad<TR>> Send<T, TR>(TPayloadType payloadType, T message)
        {
            var identifier = new MessageIdentifier(this.connectionId, Interlocked.Increment(ref this.correlationId));
            this.outgoingConnection.Send(new Message<TPayloadType, TBody>
            {
                Body = this.encoder.Encode(message),
                MessageIdentifier = identifier,
                MessageType = MessageType.Request,
                PayloadType = payloadType,
                UserId = Guid.NewGuid(),
            });
            var node = new PendingRequestNode<TR>(); 
            this.pendingRequestNodes[identifier] = node;
            return node.task.Task;
        }

        public bool Handle(Message<TPayloadType, TBody> message)
        {
            switch (message.MessageType)
            {
                case MessageType.Event:
                    if (this.events.TryGetValue(message.PayloadType, out var node))
                    {
                        node.Handle(
                            message,
                            this.encoder);
                    }
                    break;

                case MessageType.Ping:
                    // TODO send back a pong.
                    break;

                case MessageType.Pong:
                    break;

                case MessageType.Reply:
                    if (this.pendingRequestNodes.TryRemove(message.MessageIdentifier, out var pendingRequestNode))
                    {
                        pendingRequestNode.Handle(
                            message,
                            this.encoder,
                            this.messageResultMonad);
                    }
                    break;

                case MessageType.Request:
                    break;

                case MessageType.Status:
                    break;
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

        private interface ISubjectNode
        {
            void Handle(Message<TPayloadType, TBody> msg, IBodyEncoder<TBody> encoder);
        }

        private class SubjectNode<T> : ISubjectNode
        {
            public readonly Subject<T> subject = new Subject<T>();

            public void Handle(Message<TPayloadType, TBody> msg, IBodyEncoder<TBody> encoder)
            {
                if (msg.MessageResultType == MessageResultType.Success)
                {
                    subject.OnNext(encoder.Decode<T>(msg.Body));
                }
            }
        }

        private interface IPendingRequestNode
        {
            void Handle(
                Message<TPayloadType, TBody> msg,
                IBodyEncoder<TBody> encoder,
                IMessageResultMonad<TPayloadType, TBody> messageResultMonad);
        }

        private class PendingRequestNode<T> : IPendingRequestNode
        {

            public readonly TaskCompletionSource<IResultMonad<T>> task = new TaskCompletionSource<IResultMonad<T>>(TaskCreationOptions.RunContinuationsAsynchronously);
            
            private readonly StopWatchThreadSafe stopWatchThreadSafe = new StopWatchThreadSafe();
            
            public void Handle(
                Message<TPayloadType, TBody> msg,
                IBodyEncoder<TBody> encoder,
                IMessageResultMonad<TPayloadType, TBody> messageResultMonad)
            {
                task.SetResult(messageResultMonad.To<T>(msg));
            }
        }
    }
}