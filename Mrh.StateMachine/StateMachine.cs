using System;
using System.Collections.Generic;

namespace Mrh.StateMachine
{
    /// <summary>
    ///     A state machine similar to the way p sharp works.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class StateMachine<TState, TEvent, TContext, TMessage> where TState:struct where TEvent:struct where TContext:IEventContext
    {
        private readonly IRetryHandle<TState> retryHandler;

        private readonly string name;

        private readonly Dictionary<TState, IState<TState, TEvent, TContext, TMessage>> stateLookup = new Dictionary<TState, IState<TState, TEvent, TContext, TMessage>>(50);

        public StateMachine(
            string name,
            IRetryHandle<TState> retryHandler)
        {
            this.name = name;
            this.retryHandler = retryHandler;
        }

        public void Transition(
            TEvent myEvent,
            TContext context,
            TMessage message)
        {
            
        }

        /// <summary>
        ///     Used to add a state to the state machine.
        /// </summary>
        /// <param name="state">The state for the state machine.</param>
        /// <returns>The state machine to provide a fluent api.</returns>
        /// <exception cref="DuplicateStateRegisterException<TState>"></exception>
        public StateMachine<TState, TEvent, TContext, TMessage> Add(IState<TState, TEvent, TContext, TMessage> state)
        {
            if (this.stateLookup.ContainsKey(state.State))
            {
                throw new DuplicateStateRegisterException<TState>(state.State);
            }

            this.stateLookup[state.State] = state;
        }

        private struct EventNode
        {
            public readonly TEvent Event;

            public readonly Dictionary<TState, TState> FromToState;

            public EventNode(
                TEvent eventType)
            {
                this.Event = eventType;
                this.FromToState = new Dictionary<TState, TState>(5);
            }

            public void AddState(TState fromState, TState toState)
            {
                if (this.FromToState.ContainsKey(fromState))
                {
                    throw new DuplicateFromToRegisteredException<TState>(
                        fromState, toState);
                }

                this.FromToState[fromState] = toState;
            }
        }
    }
}