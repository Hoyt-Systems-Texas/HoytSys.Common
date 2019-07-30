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
    public abstract class AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId> : IDisposable
        where TState : struct
        where TEvent : struct
        where TCtx : AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private const int IDLE = 0;
        private const int RUNNING = 1;
        private const int RETRY = 2;

        public readonly TKey StateMachineKey;
        private readonly SkipQueue<EventActionNodePersist<TKey, TState, TEvent, TParam, TCtx, TUserId>> skipQueue;
        private readonly Subject<int> subject = new Subject<int>();
        private int currentRunningState = IDLE;
        private int retryCount = 0;
        private EventActionNodePersist<TKey, TState, TEvent, TParam, TCtx, TUserId> retryEvent;
        private readonly IEventPersistedStore<TKey, TState, TEvent, TParam, TCtx, TUserId> eventPersistedStore;
        private EventActionNodePersist<TKey, TState, TEvent, TParam, TCtx, TUserId> currentEventNode;

        public AbstractStateMachinePersistCtx(
            TKey stateMachineKey,
            TState currentState,
            uint size,
            IEventPersistedStore<TKey, TState, TEvent, TParam, TCtx, TUserId> eventPersistedStore)
        {
            this.CurrentState = currentState;
            this.StateMachineKey = stateMachineKey;
            this.skipQueue = new SkipQueue<EventActionNodePersist<TKey, TState, TEvent, TParam, TCtx, TUserId>>(size);
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
        public bool Next(out EventActionNodePersist<TKey, TState, TEvent, TParam, TCtx, TUserId> actionNode)
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
                this.currentEventNode = this.retryEvent;
                this.retryEvent = null;
                return true;
            }

            actionNode = null;
            return false;
        }

        public async Task<bool> Add(EventActionNodePersist<TKey, TState, TEvent, TParam, TCtx, TUserId> actionNode)
        {
            try
            {
                if (actionNode.RunOnThread)
                {
                    return this.skipQueue.Add(actionNode);
                }
                else
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
            }
            catch (Exception ex)
            {
                log.Error(ex, ex.Message);
                // TODO Come up with a retry idea but ordering is important so this could be difficult.
                return false;
            }
        }

        public bool Skip(EventActionNodePersist<TKey, TState, TEvent, TParam, TCtx, TUserId> actionNode)
        {
            var added = this.skipQueue.AddDefer(actionNode);
            // Need to do this here since skip is a special event.
            Volatile.Write(ref this.currentRunningState, IDLE);
            if (!added)
            {
                this.eventPersistedStore.SaveResult(actionNode.Id, EventResultType.Full);
                log.Warn($"Skip queue is full. State machine id {actionNode.StateMachineKey} {actionNode.Id}");
            }

            return added;
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
                        if (!this.currentEventNode.RunOnThread)
                        {
                            await this.eventPersistedStore.SaveResult(
                                this.currentEventNode.Id,
                                EventResultType.Completed);
                        }
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

        public void RetryEvent(
            EventActionNodePersist<TKey, TState, TEvent, TParam, TCtx, TUserId> eventActionNodePersist)
        {
            Interlocked.Increment(ref this.retryCount);
            Volatile.Write(ref this.retryEvent, eventActionNodePersist);
            this.currentEventNode = null;
            // TODO add a delay.  Will need to use either a retry data structure or just use a timer but there is a limit to the amount of timers.
        }
    }
}