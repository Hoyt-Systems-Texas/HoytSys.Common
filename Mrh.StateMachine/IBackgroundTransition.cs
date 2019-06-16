namespace Mrh.StateMachine
{
    /// <summary>
    ///     Used to background a transition.  Need so the state machine can support mutliple
    /// threading models.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public interface IBackgroundTransition<TState, TEvent, TContext, TMessage>
        where TState : struct
        where TEvent : struct
        where TContext : IEventContext<TState>
    {
        /// <summary>
        ///     Called to background a transition.
        /// </summary>
        /// <param name="event">The event we are transition to.</param>
        /// <param name="context">The context we are using.</param>
        /// <param name="message">The message to pass along.</param>
        void Transition(
            TEvent @event,
            TContext context,
            TMessage message);
    }
}