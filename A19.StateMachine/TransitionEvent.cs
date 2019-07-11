namespace A19.StateMachine
{
    public struct TransitionEvent<TState, TEvent, TContext, TMessage> where TState: struct where TEvent: struct where TContext:IEventContext<TState>
    {

        public readonly TState State;

        public readonly TEvent Event;

        public readonly TransitionResultType Result;

        public readonly TContext EventContext;

        public readonly TMessage Message;

        public TransitionEvent(
            TState state,
            TEvent eventType,
            TransitionResultType result,
            TContext context,
            TMessage message)
        {
            this.State = state;
            this.Event = eventType;
            this.Result = result;
            this.EventContext = context;
            this.Message = message;
        }
    }
}