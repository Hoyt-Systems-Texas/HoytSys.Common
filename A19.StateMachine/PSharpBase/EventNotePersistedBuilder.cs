using System.Collections.Generic;
using A19.Concurrent.StateMachine;

namespace A19.StateMachine.PSharpBase
{
    public class EventNotePersistedBuilder<TKey, TState, TEvent, TCtx, TParam, TUserId>
        where TState : struct
        where TEvent : struct
        where TCtx : AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        public readonly List<EventNodePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>> events =
            new List<EventNodePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>>(10);

        public EventNotePersistedBuilder<TKey, TState, TEvent, TCtx, TParam, TUserId> GoTo(
            TEvent @event,
            TState newState)
        {
            this.events.Add(
                new EventNodePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>(
                    @event,
                    EventActionType.Goto,
                    newState,
                    null));
            return this;
        }
        
        public EventNotePersistedBuilder<TKey, TState, TEvent, TCtx, TParam, TUserId> Defer(
            TEvent @event)
        {
            this.events.Add(
                new EventNodePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>(
                    @event,
                    EventActionType.Defer,
                    null,
                    null));
            return this;
        }
        
        public EventNotePersistedBuilder<TKey, TState, TEvent, TCtx, TParam, TUserId> Ignore(
            TEvent @event)
        {
            this.events.Add(
                new EventNodePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>(
                    @event,
                    EventActionType.Ignore,
                    null,
                    null));
            return this;
        }
        
        public EventNotePersistedBuilder<TKey, TState, TEvent, TCtx, TParam, TUserId> Do(
            TEvent @event,
            IAction<TKey, TState, TEvent, TCtx, TParam, TUserId> action)
        {
            this.events.Add(
                new EventNodePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>(
                    @event,
                    EventActionType.Do,
                    null,
                    action));
            return this;
        }

        public IEnumerable<EventNodePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>> Build()
        {
            return this.events;
        }
    }
}