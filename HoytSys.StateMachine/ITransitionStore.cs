using System.Threading.Tasks;

namespace A19.StateMachine
{
    /// <summary>
    ///     Used to store the transition information into a persisted store.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public interface ITransitionStore<TState, TEvent, TContext, TMessage> 
        where TState:struct 
        where TEvent:struct
        where TContext:IEventContext<TState>
    {
        /// <summary>
        ///     Used to save a transition
        /// </summary>
        /// <param name="result">The result to save to the database.</param>
        /// <param name="state">The new state.</param>
        /// <param name="myEvent">The next event to run.</param>
        /// <param name="context">The context associated with change.</param>
        /// <param name="message">The message associated with the change.</param>
        /// <returns>The Task that does the update.</returns>
        Task Save(
            TransitionResultType result,
            TState state,
            TEvent? myEvent,
            TContext context,
            TMessage message);

        Task Save(
            TMessage message);

    }
}