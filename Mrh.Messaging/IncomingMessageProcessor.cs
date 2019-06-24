using System;
using System.Threading;
using Mrh.Concurrent;
using Mrh.Core;

namespace Mrh.Messaging
{
    public class IncomingMessageProcessor<TPayloadType, TBody> : IStartable, IStoppable where TPayloadType : struct
    {

        private const int STOPPED = 0;
        private const int STARTING = 1;
        private const int RUNNING = 2;
        private const int STOPPING = 10;

        private readonly MpmcRingBuffer<MessageEnvelope<TPayloadType, TBody>> buffer =
            new MpmcRingBuffer<MessageEnvelope<TPayloadType, TBody>>(0x1000);
        private readonly IMessageStore<TPayloadType, TBody> messageStore;
        private readonly IMessageRouter<TPayloadType, TBody> messageRouter;
        private Thread processingThread;

        private int processorCurrentState;

        public IncomingMessageProcessor(
            IMessageStore<TPayloadType, TBody> messageStore,
            IMessageRouter<TPayloadType, TBody> messageRouter)
        {
            this.messageStore = messageStore;
            this.messageRouter = messageRouter;
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
            while (Volatile.Read(ref this.processorCurrentState) == RUNNING)
            {
                try
                {
                    
                }
                catch (Exception ex)
                {
                    
                }
            }
        }
    }
}