namespace A19.StateMachine.PSharpBase.Monad
{
    public interface IStateMachineMonadSettings
    {
        /// <summary>
        ///     The timeout for a node in milliseconds.
        /// </summary>
        long TimeOutMs { get; }
        
        /// <summary>
        ///     The amount of time that goes by before running the cleaner.
        /// </summary>
        long CleanAfterMs { get; }
        
        /// <summary>
        ///     The id for the server.  Only used if distributed.
        /// </summary>
        int ServerId { get; }
    }
}