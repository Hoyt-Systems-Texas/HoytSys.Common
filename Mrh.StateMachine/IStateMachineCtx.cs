namespace Mrh.StateMachine
{
    public interface IStateMachineCtx<TState> where TState:struct
    {
        
        /// <summary>
        ///     The current state of the state machine.
        /// </summary>
        TState CurrentState { get; set; }
        
        /// <summary>
        ///     The current retry count.
        /// </summary>
        int RetryCount { get; }

        /// <summary>
        ///     Used to increment the retry count.
        /// </summary>
        /// <returns>The new retry count for the node.</returns>
        int IncRetry();

    }
}