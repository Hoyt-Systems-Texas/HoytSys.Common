using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using A19.Concurrent;

namespace A19.StateMachine.PSharpBase
{
    /// <summary>
    ///     A state machine ctx.
    /// </summary>
    /// <typeparam name="TKey">The type for the key.</typeparam>
    /// <typeparam name="TEvent">The type for the event.</typeparam>
    /// <typeparam name="TParam">The type for the parameter.</typeparam>
    public abstract class AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam> : IDisposable
    {
        private const int IDLE = 0;
        private const int RUNNING = 1;
        private const int RETRY = 2;

        public readonly TKey StateMachineKey;
        private readonly SkipQueue<EventActionNodePersist<TKey, TEvent, TParam>> skipQueue;
        private readonly Subject<int> subject = new Subject<int>();
        private int currentRunningState = IDLE;
        private int retryCount = 0;
        private EventActionNodePersist<TKey, TEvent, TParam> retryEvent;
        
        public AbstractStateMachinePersistCtx(
            TKey stateMachineKey,
            TState currentState,
            uint size)
        {
            this.CurrentState = currentState;
            this.StateMachineKey = stateMachineKey;
            this.skipQueue = new SkipQueue<EventActionNodePersist<TKey, TEvent, TParam>>(size);
        }

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
        public bool Next(out EventActionNodePersist<TKey, TEvent, TParam> actionNode)
        {
            if (Interlocked.CompareExchange(
                    ref this.currentRunningState,
                    RUNNING,
                    IDLE) == IDLE)
            {
                return this.skipQueue.Next(out actionNode);
            } else if (Interlocked.CompareExchange(
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

        public bool Add(EventActionNodePersist<TKey, TEvent, TParam> actionNode)
        {
            return this.skipQueue.Add(actionNode);
        }

        public bool Skip(EventActionNodePersist<TKey, TEvent, TParam> actionNode)
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

        public void DoneWithEvent()
        {
            Volatile.Write(ref this.currentRunningState, IDLE);
        }

        public void RetryEvent(EventActionNodePersist<TKey, TEvent, TParam> eventActionNodePersist)
        {
            Interlocked.Increment(ref this.retryCount);
            Volatile.Write(ref this.retryEvent, eventActionNodePersist);
        }
    }
}