using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using A19.Concurrent;
using NLog;

namespace A19.StateMachine.PSharpBase
{
    /// <summary>
    ///     A state machine ctx.
    /// </summary>
    /// <typeparam name="TKey">The type for the key.</typeparam>
    /// <typeparam name="TEvent">The type for the event.</typeparam>
    /// <typeparam name="TParam">The type for the parameter.</typeparam>
    public abstract class AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TUserId> : IDisposable
        where TState : struct
        where TEvent : struct
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private const int IDLE = 0;
        private const int RUNNING = 1;
        private const int RETRY = 2;

        public readonly TKey StateMachineKey;
        private readonly SkipQueue<EventActionNodePersist<TKey, TEvent, TParam, TUserId>> skipQueue;
        private readonly Subject<int> subject = new Subject<int>();
        private int currentRunningState = IDLE;
        private int retryCount = 0;
        private EventActionNodePersist<TKey, TEvent, TParam, TUserId> retryEvent;
        private readonly IEventPersistedStore<TKey, TEvent, TParam, TUserId> eventPersistedStore;
        private EventActionNodePersist<TKey, TEvent, TParam, TUserId> currentEventNode;

        public AbstractStateMachinePersistCtx(
            TKey stateMachineKey,
            TState currentState,
            uint size,
            IEventPersistedStore<TKey, TEvent, TParam, TUserId> eventPersistedStore)
        {
            this.CurrentState = currentState;
            this.StateMachineKey = stateMachineKey;
            this.skipQueue = new SkipQueue<EventActionNodePersist<TKey, TEvent, TParam, TUserId>>(size);
            this.eventPersistedStore = eventPersistedStore;
        }

        /// <summary>
        ///     The id of the last state transition.
        /// </summary>
        public long LastTransitionId { get; set; }

        public IObservable<int> NewEvent
        {
            get { return this.subject.AsObservable(); }
        }

        public TState CurrentState { get; set; }

        /// <summary>
        ///     Used to get the event out of the node.
        /// </summary>
        /// <param name="actionNode">The action node to process.</param>
        /// <returns></returns>
        public bool Next(out EventActionNodePersist<TKey, TEvent, TParam, TUserId> actionNode)
        {
            if (Interlocked.CompareExchange(
                    ref this.currentRunningState,
                    RUNNING,
                    IDLE) == IDLE)
            {
                if (this.skipQueue.Next(out actionNode))
                {
                    this.currentEventNode = actionNode;
                    return true;
                }

                return false;
            }
            else if (Interlocked.CompareExchange(
                         ref this.currentRunningState,
                         RUNNING,
                         RETRY) == RETRY)
            {
                actionNode = this.retryEvent;
                this.retryEvent = null;
                return true;
            }

            actionNode = null;
            return false;
        }

        public async Task<bool> Add(EventActionNodePersist<TKey, TEvent, TParam, TUserId> actionNode)
        {
            try
            {
                await this.eventPersistedStore.Save(actionNode);
                var able = this.skipQueue.Add(actionNode);
                if (able)
                {
                    return true;
                }
                else
                {
                    await this.eventPersistedStore.SaveResult(actionNode.Id, EventResultType.Full);
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, ex.Message);
                // TODO Come up with a retry idea but ordering is important so this could be difficult.
                return false;
            }
        }

        public bool Skip(EventActionNodePersist<TKey, TEvent, TParam, TUserId> actionNode)
        {
            return this.skipQueue.AddDefer(actionNode);
        }

        public void ResetSkip()
        {
            this.skipQueue.Reset();
        }

        public virtual void Dispose()
        {
            this.subject.Dispose();
        }

        /// <summary>
        ///     Called when you are done with the event.
        /// </summary>
        /// <returns>The task that performs the update.</returns>
        public async Task DoneWithEvent()
        {
            while (true)
            {
                try
                {
                    if (this.currentEventNode == null)
                    {
                        log.Warn($"The current event node is null.  This should never happen.");
                    }
                    else
                    {
                        await this.eventPersistedStore.SaveResult(
                            this.currentEventNode.Id,
                            EventResultType.Completed);
                        this.currentEventNode = null;
                    }
                    Volatile.Write(ref this.currentRunningState, IDLE);
                }
                catch (Exception ex)
                {
                    log.Error(ex, ex.Message);
                    Thread.Sleep(5);
                }
            }
        }

        public void RetryEvent(EventActionNodePersist<TKey, TEvent, TParam, TUserId> eventActionNodePersist)
        {
            Interlocked.Increment(ref this.retryCount);
            Volatile.Write(ref this.retryEvent, eventActionNodePersist);
        }
    }
}