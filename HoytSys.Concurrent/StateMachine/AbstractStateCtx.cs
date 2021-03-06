using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace A19.Concurrent.StateMachine
{
    /// <summary>
    ///     The base context for the state machine.
    /// </summary>
    /// <typeparam name="TState">The state.</typeparam>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <typeparam name="TParam">The type for the param.</typeparam>
    public abstract class AbstractStateCtx<TState, TEvent, TParam> : IDisposable
        where TState: struct
        where TEvent: struct
    {
        private const int IDLE = 0;
        private const int RUNNING = 1;

        private int currentRunningState;
        
        private readonly SkipQueue<EventActionNode> eventQueue = new SkipQueue<EventActionNode>();
            
        private readonly Subject<int> newEventSubject = new Subject<int>();

        /**
         * raised when a new event has been added.
         */
        public IObservable<int> NewEvent
        {
            get { return this.newEventSubject.AsObservable(); }
        }
        
        public class EventActionNode
        {
            public readonly TEvent @Event;
            public readonly TParam Param;

            public EventActionNode(
                TEvent @event,
                TParam param)
            {
                this.Event = @event;
                this.Param = param;
            }
        }
        
        public TState CurrentState { get; set; }

        public void Dispose()
        {
            this.newEventSubject.Dispose();
        }

        public bool Add(
            TEvent @event,
            TParam param)
        {
            if (this.eventQueue.Add(new EventActionNode(@event, param)))
            {
                this.newEventSubject.OnNext(1);
                return true;
            }

            return false;
        }

        public bool Next(out EventActionNode actionNode)
        {
            if (Interlocked.CompareExchange(
                ref this.currentRunningState,
                RUNNING,
                IDLE) == IDLE)
            {
                return this.eventQueue.Next(out actionNode);
            }

            actionNode = null;
            return false;
        }

        public bool Skip(EventActionNode node)
        {
            return this.eventQueue.AddDefer(node);
        }

        public void ResetSkip()
        {
            this.eventQueue.Reset();
        }

        /**
         * Called when you are done processing an event.
         */
        public void DoneWithEvent()
        {
            Volatile.Write(ref this.currentRunningState, IDLE);
        }
    }
}