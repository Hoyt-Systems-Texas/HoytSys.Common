using System;
using System.Text;
using System.Threading;
using Mrh.Core;
using NetMQ;
using NetMQ.Sockets;
using NLog;

namespace Mrh.Messaging.NetMq
{
    /// <summary>
    ///     Used to process incoming connections.  Uses a NeqMQ push socket to handle the messages.
    /// </summary>
    public class IncomingConnection<TPayloadType, TBody, TMsgCtx> : IStartable, IStoppable 
        where TPayloadType:struct 
        where TMsgCtx : MessageCtx<TPayloadType, TBody>, new()
    {

        private int currentState = 0;
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly IIncomingMessageBuilder<TPayloadType, TBody> incomingMessageBuilder;
        private readonly byte[] zero = new byte[32];

        private const int STOPPED = 0;
        private const int STARTING = 1;
        private const int RUNNING = 2;
        private const int STOPPING = 10;

        private readonly string connectionString;
        private PullSocket pullSocket;
        private readonly IEncoder<TPayloadType, TBody> encoder;
        private readonly IncomingMessageProcessor<TPayloadType, TBody, TMsgCtx> incomingMessageProcessor;

        public IncomingConnection(
            INetMqConfig netMqConfig,
            IIncomingMessageBuilder<TPayloadType, TBody> incomingMessageBuilder,
            IncomingMessageProcessor<TPayloadType, TBody, TMsgCtx> incomingMessageProcessor,
            IEncoder<TPayloadType, TBody> encoder)
        {
            this.connectionString = netMqConfig.IncomingConnection;
            this.incomingMessageBuilder = incomingMessageBuilder;
            this.encoder = encoder;
            this.incomingMessageProcessor = incomingMessageProcessor;
        }
        
        public void Start()
        {
            if (Interlocked.CompareExchange(
                    ref this.currentState,
                    STARTING,
                    STOPPED) == STARTING)
            {
                this.Main();
            }
        }

        public void Stop()
        {
            Interlocked.CompareExchange(
                ref this.currentState,
                STOPPING,
                RUNNING);
        }

        private void Main()
        {
            Volatile.Write(ref this.currentState, RUNNING);
            Msg msg = new Msg();
            using (this.pullSocket = new PullSocket(this.connectionString))
            {
                while (Volatile.Read(ref this.currentState) == RUNNING)
                {
                    try
                    {
                        if (pullSocket.TryReceive(ref msg, new TimeSpan(0, 0, 5)))
                        {
                            if (msg.Data.Length >= MessageSettings.HEADER_SIZE
                                && this.encoder.Decode(msg.Data, out MessageEnvelope<TPayloadType, TBody> envelope))
                            {
                                if (this.incomingMessageBuilder.Add(envelope, out Message<TPayloadType, TBody> message))
                                {
                                    this.incomingMessageProcessor.Handle(message);
                                }
                            }
                            else
                            {
                                log.Warn($"Invalid message received");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, ex.Message);
                    }
                }
            }
            Volatile.Write(ref this.currentState, STOPPED);
        }
    }
}