namespace Mrh.StateMachine
{
    public interface IRetryHandle<TState> where TState: struct
    {
        /// <summary>
        ///     Called when we want to retry a state transition.
        /// </summary>
        /// <param name="ctx">The context to retry.</param>
        void Retry(IStateMachineCtx<TState> ctx);
    }
}