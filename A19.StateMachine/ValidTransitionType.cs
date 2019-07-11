namespace A19.StateMachine
{
    public enum ValidTransitionType
    {
        /// <summary>
        ///     Should execute the transition of the state.
        /// </summary>
        Transition = 0,
        /// <summary>
        ///     Ignore the request when it's a specific event.  Module after PSharps ability to ignore specific transitions.
        /// </summary>
        Ignore = 1
    }
}