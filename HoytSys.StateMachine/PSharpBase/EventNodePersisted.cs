using A19.Concurrent.StateMachine;

namespace A19.StateMachine.PSharpBase
{
    public class EventNodePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        public readonly TEvent Event;
        public readonly EventActionType EventAction;
        public readonly TState? State;
        public readonly IAction<TKey, TState, TEvent, TCtx, TParam, TUserId> Action;

        public EventNodePersisted(
            TEvent @event,
            EventActionType eventAction,
            TState? state,
            IAction<TKey, TState, TEvent, TCtx, TParam, TUserId> action)
        {
            this.Event = @event;
            this.EventAction = eventAction;
            this.State = state;
            this.Action = action;
        }
    }
}