using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mrh.StateMachine
{
    /// <summary>
    ///     A state machine similar to the way p sharp works.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class StateMachine<TState, TEvent, TContext, TMessage> where TState : struct
        where TEvent : struct
        where TContext : IEventContext
    {
        private readonly IRetryHandle<TState> retryHandler;

        private readonly string name;

        private readonly Dictionary<TState, IState<TState, TEvent, TContext, TMessage>> stateLookup =
            new Dictionary<TState, IState<TState, TEvent, TContext, TMessage>>(50);

        private readonly Dictionary<TEvent, EventNode> eventLookup = new Dictionary<TEvent, EventNode>(100);

        private readonly ITransitionStore<TState, TEvent, TContext, TMessage> transitionStore;

        public StateMachine(
            string name,
            IRetryHandle<TState> retryHandler,
            ITransitionStore<TState, TEvent, TContext, TMessage> transitionStore)
        {
            this.name = name;
            this.retryHandler = retryHandler;
            this.transitionStore = transitionStore;
        }

        /// <summary>
        ///     Transition to a new state.
        /// </summary>
        /// <param name="myEvent">The event that has been triggered.</param>
        /// <param name="context">The message context.</param>
        /// <param name="message">Any information to go along with the message.</param>
        public async Task Transition(
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
            foreach (var transitionInfo in state.SupportedTransitions)
            {
                EventNode node;
                if (!this.eventLookup.TryGetValue(transitionInfo.Event, out node))
                {
                    node = new EventNode(transitionInfo.Event);
                    this.eventLookup[transitionInfo.Event] = node;
                }
                node.AddState(state.State, transitionInfo.ToState);
            }

            return this;
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