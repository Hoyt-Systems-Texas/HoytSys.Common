using System.Collections.Generic;
using System.Threading.Tasks;

namespace A19.StateMachine
{
    public interface IState<TState, TEvent, TEventContext, TMessage> where TState: struct where TEvent: struct where TEventContext:IEventContext<TState>
    {
        
        /// <summary>
        ///     The state this handles for.
        /// </summary>
        TState State { get; }

        IEnumerable<ValidTransition<TState, TEvent>> SupportedTransitions { get; }
        
        /// <summary>
        ///     Called when we enter a state.
        /// </summary>
        Task<TransitionResult<TEvent>> Entry(TEvent changeEvent, TEventContext eventContext, TMessage message);

        /// <summary>
        ///     Called when we exit a state.
        /// </summary>
        Task Exit(TEvent changeEvent, TEventContext eventContext, TMessage message);

    }
}