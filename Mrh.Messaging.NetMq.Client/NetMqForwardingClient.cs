using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using Mrh.Concurrent;
using Mrh.Messaging.Client;
using Mrh.Messaging.Common;
using NetMQ;
using NetMQ.Sockets;
using NLog;

namespace Mrh.Messaging.NetMq.Client
{
    public class NetMqForwardingClient<TPayloadType, TBody> : IForwardingClient<TPayloadType, TBody>
        where TPayloadType : struct
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly MpmcRingBuffer<MessageEnvelope<TPayloadType, TBody>> sendingQueue =
            new MpmcRingBuffer<MessageEnvelope<TPayloadType, TBody>>(0x1000);

        private const int STOPPED = 0;
        private const int STARTING = 1;
        private const int RUNNING = 2;
        private const int STOPPING = 10;
        private int receivingState = 0;
        private int sendingState = 0;

        private readonly int maxFrameSize;

        private readonly Subject<MessageEnvelope<TPayloadType, TBody>> subject =
            new Subject<MessageEnvelope<TPayloadType, TBody>>();

        private Thread sendThread;
        private Thread receiveThread;
        private string incomingConnection;
        private string outgoingConnection;
        private readonly IEncoder<TPayloadType, TBody> encoder;

        public NetMqForwardingClient(
            IMessageSetting messageSetting,
            INetMqConfig netMqConfig,
            IEncoder<TPayloadType, TBody> encoder)
        {
            this.maxFrameSize = messageSetting.MaxFrameSize;
            this.incomingConnection = netMqConfig.IncomingConnection;
            this.outgoingConnection = netMqConfig.OutgoingConnection;
            this.encoder = encoder;
        }

        public void Send(MessageEnvelope<TPayloadType, TBody> messageEnvelope)
        {
            throw new NotImplementedException();
        }

        public IObservable<MessageEnvelope<TPayloadType, TBody>> Receive
        {
            get { return this.subject; }
        }

        public void Start()
        {
            if (Interlocked.CompareExchange(ref this.receivingState, STARTING, STOPPED) == STOPPED)
            {
                this.receiveThread = new Thread(this.ReceivingMain)
                {
                    Name = "Receiving Thread",
                    IsBackground = true
                };
            }

            if (Interlocked.CompareExchange(ref this.sendingState, STARTING, STOPPED) == STOPPED)
            {
                this.sendThread = new Thread(this.SendingMain)
                {
                    Name =  "Sending Thread",
                    IsBackground = true
                };
            }
        }

        public void Stop()
        {
            Interlocked.CompareExchange(ref this.sendingState, STOPPING, RUNNING);
            Interlocked.CompareExchange(ref this.receivingState, STOPPING, RUNNING);
        }

        private void ReceivingMain()
        {
            log.Info("Started the receive thread.");
            Volatile.Write(ref this.receivingState, RUNNING);
            var msg = new Msg();
            msg.InitPool(this.maxFrameSize);
            using (var pullSocket = new PullSocket(this.incomingConnection))
            {
                while (Volatile.Read(ref this.receivingState) == RUNNING)
                {
                    try
                    {
                        if (pullSocket.TryReceive(ref msg, new TimeSpan(0, 0, 5)))
                        {
                            var body = msg.Data;
                            if (this.encoder.Decode(body, out var envelope))
                            {
                                this.subject.OnNext(envelope);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, ex.Message);
                    }
                }
            }

            Volatile.Write(ref this.receivingState, STOPPED);
            log.Info("Receiving thread has stopped.");
        }

        private void SendingMain()
        {
            log.Info("Started the sending thread.");
            Volatile.Write(ref this.sendingState, RUNNING);
            var msg = new Msg();
            msg.InitPool(this.maxFrameSize);
            var spinWait = new SpinWait();
            using (var pushSocket = new PushSocket(this.outgoingConnection))
            {
                while (Volatile.Read(ref this.sendingState) == RUNNING)
                {
                    try
                    {
                        var body = msg.Data;
                        if (this.sendingQueue.TryPoll(out var envelope))
                        {
                            this.encoder.Encode(envelope, ref body);
                            if (!pushSocket.TrySend(ref msg, new TimeSpan(0, 0, 15), false))
                            {
                                log.Error("Unable to send a message.");
                            }
                        }
                        else
                        {
                            spinWait.SpinOnce();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, ex.Message);
                    }
                }
            }

            Volatile.Write(ref this.sendingState, STOPPED);
            log.Info("Sending thread has stopped.");
        }
    }
}