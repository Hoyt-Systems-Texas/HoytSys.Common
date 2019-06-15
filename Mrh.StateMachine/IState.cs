using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mrh.StateMachine
{
    public interface IState<TState, TEvent, TEventContext, TMessage> where TState: struct where TEvent: struct where TEventContext:IEventContext
    {
        
        /// <summary>
        ///     The state this handles for.
        /// </summary>
        TState State { get; }

        IEnumerable<ValidTransition<TState, TEvent>> SupportedTransitions { get; }
        
        /// <summary>
        ///     Called when we enter a state.
        /// </summary>
        Task Entry(TEvent changeEvent, TEventContext eventContext, TMessage message);

        /// <summary>
        ///     Called when we exit a state.
        /// </summary>
        Task Exit(TEvent changeEvent, TEventContext eventContext, TMessage message);

    }
}