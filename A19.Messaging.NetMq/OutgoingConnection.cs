using System;
using System.Threading;
using System.Threading.Tasks;
using A19.Concurrent;
using A19.Messaging.Common;
using NetMQ;
using NetMQ.Sockets;
using NLog;

namespace A19.Messaging.NetMq
{
    /// <summary>
    ///     Used to send an outgoing message.
    /// </summary>
    /// <typeparam name="TPayloadType">The type for the payload.</typeparam>
    /// <typeparam name="TBody">The type for the body.</typeparam>
    public class OutgoingConnection<TPayloadType, TBody> : IOutgoingConnection<TPayloadType, TBody> where TPayloadType:struct
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        
        private const int STOPPED = 0;
        private const int STARTING = 1;
        private const int RUNNING = 2;
        private const int STOPPING = 10;

        private Thread processingThread;
        private int currentState = STOPPED;
        private readonly string connectionString;
        private PushSocket pushSocket;
        private readonly IEnvelopFactory<TPayloadType, TBody> envelopFactory;
        private readonly MpmcRingBuffer<IMessageSend> buffer = new MpmcRingBuffer<IMessageSend>(0x1000);
        private readonly IEncoder<TPayloadType, TBody> encoder;
        private readonly int maxFrameSize;

        public OutgoingConnection(
            INetMqConfig netMqConfig,
            IEnvelopFactory<TPayloadType, TBody> envelopFactory,
            IMessageSetting messageSetting,
            IEncoder<TPayloadType, TBody> encoder)
        {
            this.connectionString = netMqConfig.OutgoingConnection;
            this.envelopFactory = envelopFactory;
            this.maxFrameSize = messageSetting.MaxFrameSize;
            this.encoder = encoder;
        }

        public void Send(Message<TPayloadType, TBody> message)
        {
            this.buffer.Offer(new SendFullMessage(message));
        }

        public void Dispose()
        {
            Interlocked.CompareExchange(ref this.currentState, RUNNING, STOPPING);
        }

        public Task Connect()
        {
            if (Interlocked.CompareExchange(ref this.currentState, STARTING, STOPPED) == STOPPED)
            {
                this.processingThread = new Thread(this.Main);
                this.processingThread.Name = "NetMQ outgoing thread";
                this.processingThread.Start();
            }

            return Task.FromResult(0);
        }

        public Task Reconnect()
        {
            return Task.FromResult(0);
        }

        private void Main()
        {
            using (this.pushSocket = new PushSocket(this.connectionString))
            {
                Volatile.Write(ref this.currentState, RUNNING);
                var spin = new SpinWait();
                var msg = new Msg();
                msg.InitPool(this.maxFrameSize);
                while (Volatile.Read(ref this.currentState) == RUNNING)
                {
                    try
                    {
                        if (this.buffer.TryPoll(out var send))
                        {
                            send.Send(
                                this.pushSocket,
                                this.envelopFactory,
                                this.encoder,
                                msg,
                                this.maxFrameSize);
                        }
                        else
                        {
                            spin.SpinOnce();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, ex.Message);
                    }
                }
            }
        }

        private interface IMessageSend
        {

            /// <summary>
            ///     Called when to send a message.
            /// </summary>
            /// <param name="pushSocket">The push socket to use to send a message.</param>
            void Send(
                PushSocket pushSocket,
                IEnvelopFactory<TPayloadType, TBody> envelopFactory,
                IEncoder<TPayloadType, TBody> encoder,
                Msg msg,
                int maxFrameSize);
        }

        private class SendFullMessage : IMessageSend
        {
            private static readonly TimeSpan timeOut = new TimeSpan(0, 0, 30);
            private readonly Message<TPayloadType, TBody> message;
            
            public SendFullMessage(
                Message<TPayloadType, TBody> message)
            {
                this.message = message;
            }

            public void Send(
                PushSocket pushSocket,
                IEnvelopFactory<TPayloadType, TBody> envelopFactory,
                IEncoder<TPayloadType, TBody> encoder,
                Msg msg,
                int maxFrameSize)
            {
                envelopFactory.CreateEnvelops(
                    this.message,
                    (envelope) =>
                    {
                        var array = msg.Data;
                        encoder.Encode(envelope, ref array);
                        if (!pushSocket.TrySend(ref msg, timeOut, false))
                        {
                            log.Error($"Failed to send a message.");
                        }
                        msg.InitPool(maxFrameSize);
                    });
            }
        }
    }
}