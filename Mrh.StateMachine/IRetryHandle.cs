namespace Mrh.StateMachine
{
    public interface IRetryHandle<TState, TEvent, TContext, TMessage> where TState: struct
    {
        /// <summary>
        ///     Called when we want to retry a state transition.
        /// </summary>
        void Retry(
            TEvent @event,
            TContext context,
            TMessage message);
    }
}