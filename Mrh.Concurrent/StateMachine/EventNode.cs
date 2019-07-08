namespace Mrh.Concurrent.StateMachine
{
    public struct EventNode<TState, TEvent, TCtx, TParam>
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateCtx<TState, TEvent, TParam>
    {
        public readonly TEvent Event;
        public readonly EventActionType EventAction;
        public readonly TState? State;
        public readonly IAction<TState, TEvent, TCtx, TParam> Action;

        public EventNode(
            TEvent @event,
            EventActionType eventAction,
            TState? state,
            IAction<TState, TEvent, TCtx, TParam> action)
        {
            this.Event = @event;
            this.EventAction = eventAction;
            this.State = state;
            this.Action = action;
        }
    }
}