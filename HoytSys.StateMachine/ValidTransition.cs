namespace A19.StateMachine
{
    /// <summary>
    ///     The possible transitions the state supports.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TEvent">The event type.</typeparam>
    public struct ValidTransition<TState, TEvent> where TState:struct where TEvent:struct
    {
        
        /// <summary>
        ///     The event that triggers the transition.
        /// </summary>
        public readonly TEvent Event;

        /// <summary>
        ///     The new state to go to.
        /// </summary>
        public readonly TState ToState;

        public readonly ValidTransitionType ValidTransitionType;

        private ValidTransition(
            ValidTransitionType validTransitionType,
            TEvent myEvent,
            TState toState)
        {
            this.ValidTransitionType = validTransitionType;
            this.Event = myEvent;
            this.ToState = toState;
        }

        /// <summary>
        ///     Executes the transition to the new state.
        /// </summary>
        /// <param name="myEvent">The event to execute.</param>
        /// <param name="state">The new state to go to.</param>
        /// <returns>The valid transition object.</returns>
        public static ValidTransition<TState, TEvent> To(
            TEvent myEvent,
            TState state)
        {
            return new ValidTransition<TState, TEvent>(
                ValidTransitionType.Transition,
                myEvent,
                state);
        }

        /// <summary>
        ///     Ignore the event.
        /// </summary>
        /// <param name="myEvent"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static ValidTransition<TState, TEvent> Ignore(
            TEvent myEvent)
        {
            return new ValidTransition<TState, TEvent>(
                ValidTransitionType.Ignore,
                myEvent,
                default);
        }
    }
}