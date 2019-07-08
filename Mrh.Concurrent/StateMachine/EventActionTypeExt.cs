namespace Mrh.Concurrent.StateMachine
{
    public static class EventActionTypeExt<
        TState,
        TEvent,
        TCtx,
        TParam>
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateCtx<TState, TEvent, TParam>
    {
        /// <summary>
        ///     Tells the state to the next machine.
        /// </summary>
        /// <param name="event">The event to handle.</param>
        /// <param name="state">THe next state to go to.</param>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TCtx"></typeparam>
        /// <typeparam name="TParam"></typeparam>
        /// <returns></returns>
        public static EventNode<TState, TEvent, TCtx, TParam> GoTo(
            TEvent @event,
            TState state)
        {
            return new EventNode<TState, TEvent, TCtx, TParam>(
                @event,
                EventActionType.Goto,
                state,
                null);
        } 
        
        public static EventNode<TState, TEvent, TCtx, TParam> Ignore(
            TEvent @event)
        {
            return new EventNode<TState, TEvent, TCtx, TParam>(
                @event,
                EventActionType.Ignore,
                null,
                null);
        }

        public static EventNode<TState, TEvent, TCtx, TParam> Do(
            TEvent @event,
            IAction<TState, TEvent, TCtx, TParam> action)
        {
            return new EventNode<TState, TEvent, TCtx, TParam>(
                @event,
                EventActionType.Do,
                null,
                action);
        }
    }
}