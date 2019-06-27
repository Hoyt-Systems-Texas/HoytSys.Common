using System;
using System.Collections.Generic;
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
    public class IncomingMessageProcessor<TPayloadType, TBody, TMsgCtx> : IIncomingMessageProcessor<TPayloadType, TBody> where TPayloadType : struct
        where TMsgCtx : MessageCtx<TPayloadType, TBody>, new()
    {
        private static ILogger log = LogManager.GetCurrentClassLogger();

        private const int STOPPED = 0;
        private const int STARTING = 1;
        private const int RUNNING = 2;
        private const int STOPPING = 10;

        private int currentState = STOPPED;

        private readonly MpmcRingBuffer<Message<TPayloadType, TBody>> buffer;
        private readonly IMessageStore<TPayloadType, TBody, TMsgCtx> messageStore;
        private readonly IMessageRouter<TPayloadType, TBody, TMsgCtx> messageRouter;
        private readonly IOutgoingConnection<TPayloadType, TBody> outgoingConnection;
        private readonly IMessageResultFactory<TBody> messageResultFactory;
        private readonly FastSemaphore semaphore;
        private Thread processingThread;

        public IncomingMessageProcessor(
            IMessageStore<TPayloadType, TBody, TMsgCtx> messageStore,
            IMessageRouter<TPayloadType, TBody, TMsgCtx> messageRouter,
            IOutgoingConnection<TPayloadType, TBody> outgoingConnection,
            IMessageResultFactory<TBody> messageResultFactory)
        {
            this.buffer = new MpmcRingBuffer<Message<TPayloadType, TBody>>(0x1000);
            this.outgoingConnection = outgoingConnection;
            this.messageResultFactory = messageResultFactory;
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

        private Message<TPayloadType, TBody> CreateReplyMsg(
            IMessageCtx<TPayloadType, TBody> ctx,
            MessageResult<TBody> result)
        {
            return new Message<TPayloadType, TBody>
            {
                Body = result.Body,
                MessageIdentifier = result.MessageIdentifier,
                MessageType = MessageType.Reply,
                PayloadType = ctx.PayloadType,
                RequestId = ctx.RequestId,
                UserId = ctx.UserId,
                MessageResultType = result.Result,
                ToConnectionId = ctx.MessageIdentifier.ConnectionId
            };
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
                                                var result = await this.messageRouter.Route(ctx);
                                                try
                                                {
                                                    result.MessageIdentifier = ctx.MessageIdentifier;
                                                    result.RequestId = ctx.RequestId;
                                                    await this.messageStore.Save(result);
                                                    this.outgoingConnection.Send(
                                                        this.CreateReplyMsg(ctx, result));
                                                }
                                                catch (Exception ex)
                                                {
                                                    log.Error(ex, ex.Message);
                                                    result = this.messageResultFactory.CreateError(new List<string>
                                                    {
                                                        "Unexpected error has occurred"
                                                    });
                                                    result.MessageIdentifier = ctx.MessageIdentifier;
                                                    result.RequestId = ctx.RequestId;
                                                    await this.messageStore.Save(result);
                                                    this.outgoingConnection.Send(
                                                        this.CreateReplyMsg(ctx, result));
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                log.Error(ex, ex.Message);
                                                // TODO send back error message.
                                                try
                                                {
                                                    var result = this.messageResultFactory.CreateError(new List<string>
                                                    {
                                                        "Unexpected error has occurred"
                                                    });
                                                    result.MessageIdentifier = ctx.MessageIdentifier;
                                                    result.RequestId = ctx.RequestId;
                                                    await this.messageStore.Save(result);
                                                    this.outgoingConnection.Send(
                                                        this.CreateReplyMsg(ctx, result));
                                                }
                                                catch (Exception ex1)
                                                {
                                                    log.Error(ex1, ex1.Message);
                                                }
                                            }
                                            finally
                                            {
                                                ctx.TrySetState(MessageState.Completed, MessageState.Running);
                                            }
                                        }
                                        else
                                        {
                                            // TODO send back a status or see if there is an result.
                                            if (ctx.CurrentState == MessageState.Completed)
                                            {
                                                var result = await this.messageStore.TryGetResult(ctx.MessageIdentifier);
                                                if (result.Item1)
                                                {
                                                    var responseMsg = this.CreateReplyMsg(
                                                        ctx,
                                                        result.Item2);
                                                    this.outgoingConnection.Send(responseMsg);
                                                }
                                            }
                                            else
                                            {
                                                var result = this.messageResultFactory.State(ctx.CurrentState);
                                                result.MessageIdentifier = ctx.MessageIdentifier;
                                                result.RequestId = ctx.RequestId;
                                                var responseMsg = this.CreateReplyMsg(
                                                    ctx,
                                                    result);
                                                this.outgoingConnection.Send(responseMsg);
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