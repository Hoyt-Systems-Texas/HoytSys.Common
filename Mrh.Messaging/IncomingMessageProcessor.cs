using System;
using System.Threading;
using System.Threading.Tasks;
using Mrh.Concurrent;
using Mrh.Core;
using NLog;

namespace Mrh.Messaging
{
    /// <summary>
    ///     Used to process message we have received everything for.
    /// </summary>
    /// <typeparam name="TPayloadType">The payload type.</typeparam>
    /// <typeparam name="TBody">The body type.</typeparam>
    public class IncomingMessageProcessor<TPayloadType, TBody, TMsgCtx> : IStartable, IStoppable where TPayloadType:struct 
    where TMsgCtx:MessageCtx<TPayloadType, TBody>, new()
    {

        private static ILogger log = LogManager.GetCurrentClassLogger();
        
        private const int STOPPED = 0;
        private const int STARTING = 1;
        private const int RUNNING = 2;
        private const int STOPPING = 10;

        private int currentState = STOPPED;
            
        private readonly MpmcRingBuffer<Message<TPayloadType, TBody>> buffer;
        private readonly IMessageStore<TPayloadType, TBody> messageStore;
        private readonly IMessageRouter<TPayloadType, TBody> messageRouter;
        private readonly IOutgoingConnection<TPayloadType, TBody> outgoingConnection;
        private readonly FastSemaphore semaphore;
        private Thread processingThread;

        public IncomingMessageProcessor(
            IMessageStore<TPayloadType, TBody> messageStore,
            IMessageRouter<TPayloadType, TBody> messageRouter,
            IOutgoingConnection<TPayloadType, TBody> outgoingConnection)
        {
            this.buffer = new MpmcRingBuffer<Message<TPayloadType, TBody>>(0x1000);
            this.messageStore = messageStore;
            this.messageRouter = messageRouter;
            this.semaphore = new FastSemaphore(100);
        }

        /// <summary>
        ///     This call can't block for any reason and should use the build in thread pool for processing messages.
        /// </summary>
        /// <returns>true if we are able to accept the message.</returns>
        public bool Handle(Message<TPayloadType, TBody> message)
        {
            return this.buffer.Offer(message);
        }

        public void Start()
        {
            if (Interlocked.CompareExchange(ref this.currentState, STARTING, STOPPED) == STOPPED)
            {
                this.processingThread = new Thread(this.Main);
                this.processingThread.Name = "IncomingMessageProcessor";
                this.processingThread.Start();
            }
        }

        public void Stop()
        {
            if (Interlocked.CompareExchange(ref this.currentState, STOPPING, RUNNING) == RUNNING)
            {
                this.processingThread.Join();
            }
        }

        private void Main()
        {
            Volatile.Write(ref this.currentState, RUNNING);
            while (Volatile.Read(ref this.currentState) == STOPPED)
            {
                try
                {
                    if (this.buffer.TryPoll(out Message<TPayloadType, TBody> msg))
                    {
                        if (this.semaphore.AcquireFailFast())
                        {
                            if (this.buffer.TryPoll(out msg))
                            {
                                Task.Run(async () =>
                                {
                                    try
                                    {
                                        var ctx = new TMsgCtx()
                                        {
                                            Body = msg.Body,
                                            MessageIdentifier = msg.MessageIdentifier,
                                            MessageType = msg.MessageType,
                                            PayloadType = msg.PayloadType,
                                            UserId = msg.UserId
                                        };
                                        await this.messageStore.Save(ctx);
                                        if (ctx.TrySetState(MessageState.Persisted, MessageState.New))
                                        {
                                            ctx.TrySetState(MessageState.Persisted, MessageState.Running);
                                            try
                                            {
                                                await this.messageRouter.Route(ctx);
                                            }
                                            catch (Exception ex)
                                            {
                                                log.Error(ex, ex.Message);
                                                // TODO send back error message.
                                                ctx.MessageResultType = MessageResultType.Error;
                                            }
                                            finally
                                            {
                                                ctx.TrySetState(MessageState.Completed, MessageState.Running);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        // TODO send back and error message.
                                        log.Error(ex, ex.Message);
                                    }
                                    finally
                                    {
                                        this.semaphore.Release();
                                    }
                                });
                            }
                            else
                            {
                                this.semaphore.Release();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, ex.Message);  
                }
            }
        }
    }
}