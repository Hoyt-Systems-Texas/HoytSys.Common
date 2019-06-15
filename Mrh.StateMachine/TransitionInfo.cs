namespace Mrh.StateMachine
{
    /// <summary>
    ///     The possible transitions the state supports.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TEvent">The event type.</typeparam>
    public struct TransitionInfo<TState, TEvent> where TState:struct where TEvent:struct
    {
        public readonly TEvent Event;

        public readonly TState NewState;

        public TransitionInfo(
            TEvent myEvent,
            TState newState)
        {
            this.Event = myEvent;
            this.NewState = newState;
        }
    }
}