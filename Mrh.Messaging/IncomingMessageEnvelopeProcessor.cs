using System;
using System.Threading;
using Mrh.Concurrent;
using Mrh.Core;

namespace Mrh.Messaging
{
    public class IncomingMessageEnvelopeProcessor<TPayloadType, TBody> : IStartable, IStoppable where TPayloadType : struct
    {
        private const int STOPPED = 0;
        private const int STARTING = 1;
        private const int RUNNING = 2;
        private const int STOPPING = 10;

        private readonly MpmcRingBuffer<MessageEnvelope<TPayloadType, TBody>> buffer =
            new MpmcRingBuffer<MessageEnvelope<TPayloadType, TBody>>(0x1000);

        private readonly IIncomingMessageBuilder<TPayloadType, TBody> incomingMessageBuilder;
        private Thread processingThread;

        private int processorCurrentState;

        public IncomingMessageEnvelopeProcessor(
            IIncomingMessageBuilder<TPayloadType, TBody> incomingMessageBuilder)
        {
            this.incomingMessageBuilder = incomingMessageBuilder;
        }

        public void Start()
        {
            this.ChangeProcessorState(RUNNING);
        }

        private void ChangeProcessorState(
            int newState)
        {
            switch (newState)
            {
                case STARTING:
                    if (this.SetState(
                        newState,
                        STOPPED))
                    {
                        this.SetupThread();
                    }

                    break;

                case STOPPING:
                    if (this.SetState(
                        newState,
                        RUNNING))
                    {
                        // Do nothing will stop on next spin.
                    }

                    break;
            }
        }

        private bool SetState(
            int newState,
            int currentState)
        {
            return Interlocked.CompareExchange(
                       ref this.processorCurrentState,
                       newState,
                       currentState) == currentState;
        }

        private void SetState(
            int newState)
        {
            Volatile.Write(ref this.processorCurrentState, newState);
        }

        public void Stop()
        {
            this.ChangeProcessorState(STOPPING);
        }

        private void SetupThread()
        {
            this.processingThread = new Thread(this.Main);
            this.processingThread.Start();
        }

        private void Main()
        {
            this.SetState(RUNNING);
            var spin = new SpinWait();
            while (Volatile.Read(ref this.processorCurrentState) == RUNNING)
            {
                try
                {
                    if (this.buffer.TryPoll(out MessageEnvelope<TPayloadType, TBody> envelope))
                    {
                        Message<TPayloadType, TBody> message;
                        if (this.incomingMessageBuilder.Add(
                            envelope,
                            out message))
                        {
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}