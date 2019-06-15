namespace Mrh.StateMachine
{
    public struct TransitionResult<TEvent>
    {
        public readonly TEvent Event;

        public readonly TransitionResultType ResultType;

        public TransitionResult(
            TEvent myEvent,
            TransitionResultType resultType)
        {
            this.Event = myEvent;
            this.ResultType = resultType;
        }

        /// <summary>
        ///     Indicates that the state machine has reached it's final state and needs to stop.
        /// </summary>
        /// <returns>The transition result.</returns>
        public static TransitionResult<TEvent> Stop()
        {
            return new TransitionResult<TEvent>(
                default(TEvent),
                TransitionResultType.Stop);
        }

        /// <summary>
        ///     Return when we need to retry.
        /// </summary>
        /// <returns>TThe transition result for a retry.</returns>
        public static TransitionResult<TEvent> Retry()
        {
            return new TransitionResult<TEvent>(
                default(TEvent),
                TransitionResultType.Retry);
        }

        /// <summary>
        ///     The next event to go to.
        /// </summary>
        /// <param name="myEvent">The next event to raise.</param>
        /// <returns>The next event to raise.</returns>
        public static TransitionResult<TEvent> To(
            TEvent myEvent)
        {
            return new TransitionResult<TEvent>(
                myEvent,
                TransitionResultType.Transition);
        }
    }
}