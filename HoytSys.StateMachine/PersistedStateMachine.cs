using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace A19.StateMachine
{
    /// <summary>
    ///     A state machine similar to the way p sharp works.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class PersistedStateMachine<TState, TEvent, TContext, TMessage> where TState : struct
        where TEvent : struct
        where TContext : IEventContext<TState>
    {
        private readonly IRetryHandle<TState, TEvent, TContext, TMessage> retryHandler;

        private readonly string name;

        private readonly Dictionary<TState, IState<TState, TEvent, TContext, TMessage>> stateLookup =
            new Dictionary<TState, IState<TState, TEvent, TContext, TMessage>>(50);

        private readonly Dictionary<TEvent, EventNode> eventLookup = new Dictionary<TEvent, EventNode>(100);

        private readonly ITransitionStore<TState, TEvent, TContext, TMessage> transitionStore;

        private readonly IBackgroundTransition<TState, TEvent, TContext, TMessage> backgroundTransition;

        private readonly Action<Exception, TEvent, TContext, TMessage> errorHandler;

        public PersistedStateMachine(
            string name,
            IRetryHandle<TState, TEvent, TContext, TMessage> retryHandler,
            ITransitionStore<TState, TEvent, TContext, TMessage> transitionStore,
            IBackgroundTransition<TState, TEvent, TContext, TMessage> background,
            Action<Exception, TEvent, TContext, TMessage> errorHandler)
        {
            this.name = name;
            this.retryHandler = retryHandler;
            this.transitionStore = transitionStore;
            this.backgroundTransition = background;
            this.errorHandler = errorHandler;
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
            await this.transitionStore.Save(message);
            var stop = false;
            while (!stop)
            {
                var currentEvent = myEvent;
                var currentState = this.GetStateNode(context.CurrentState);
                var eventNode = this.GetEventNode(currentEvent);
                await this.transitionStore.Save(
                    TransitionResultType.Transition,
                    context.CurrentState,
                    myEvent,
                    context,
                    message);
                try
                {
                    var transition = eventNode.Get(context.CurrentState);
                    var result = await transition.Run(
                        this.stateLookup,
                        currentState,
                        context,
                        message);
                    switch (result.ResultType)
                    {
                        case TransitionResultType.Transition:
                            currentEvent = result.Event;
                            break;
                        
                        case TransitionResultType.TransitionBackground:
                            this.backgroundTransition.Transition(
                                result.Event,
                                context,
                                message);
                            stop = true;
                            break;
                        
                        case TransitionResultType.Retry:
                            await this.transitionStore.Save(
                                TransitionResultType.Retry,
                                currentState.State,
                                myEvent,
                                context,
                                message);
                            this.retryHandler.Retry(
                                myEvent,
                                context,
                                message);
                            stop = true;
                            break;
                        
                        case TransitionResultType.Stop:
                            await this.transitionStore.Save(
                                TransitionResultType.Stop,
                                context.CurrentState,
                                null,
                                context,
                                message);
                            stop = true;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    this.errorHandler(
                        ex,
                        currentEvent,
                        context,
                        message);
                    return;
                }
            }
        }
        
        /// <summary>
        ///     Used to add a state to the state machine.
        /// </summary>
        /// <param name="state">The state for the state machine.</param>
        /// <returns>The state machine to provide a fluent api.</returns>
        /// <exception cref="DuplicateStateRegisterException<TState>"></exception>
        public PersistedStateMachine<TState, TEvent, TContext, TMessage> Add(IState<TState, TEvent, TContext, TMessage> state)
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
                node.AddState(state.State, this.From(transitionInfo));
            }

            return this;
        }

        private EventNode GetEventNode(TEvent myEvent)
        {
            EventNode node;
            if (this.eventLookup.TryGetValue(myEvent, out node))
            {
                return node;
            }
            else
            {
                throw new UnableToFindEventException<TEvent>(myEvent);
            }
        }

        /// <summary>
        ///     Used to get a state node.
        /// </summary>
        /// <param name="state">The state node.</param>
        /// <returns></returns>
        /// <exception cref="UnableToFindStateException<TState>"></exception>
        private IState<TState, TEvent, TContext, TMessage> GetStateNode(TState state)
        {
            IState<TState, TEvent, TContext, TMessage> stateObject;
            if (this.stateLookup.TryGetValue(
                state,
                out stateObject))
            {
                return stateObject;
            }
            throw new UnableToFindStateException<TState>(state);
        }

        private struct EventNode
        {
            public readonly TEvent Event;

            public readonly Dictionary<TState, ITransitionNode> FromToState;

            public EventNode(
                TEvent eventType)
            {
                this.Event = eventType;
                this.FromToState = new Dictionary<TState, ITransitionNode>(5);
            }

            public void AddState(TState fromState, ITransitionNode toState)
            {
                if (this.FromToState.ContainsKey(fromState))
                {
                    throw new DuplicateFromToRegisteredException<TState>(
                        fromState);
                }

                this.FromToState[fromState] = toState;
            }

            public ITransitionNode Get(TState currentState)
            {
                ITransitionNode node;
                if (this.FromToState.TryGetValue(currentState, out node))
                {
                    return node;
                }
                throw new UnableToFindStateException<TState>(currentState);
            }
        }

        private interface ITransitionNode
        {
            Task<TransitionResult<TEvent>> Run(
                Dictionary<TState, IState<TState, TEvent, TContext, TMessage>> stateLookup,
                IState<TState, TEvent, TContext, TMessage> currentState,
                TContext context,
                TMessage message);
        }

        /// <summary>
        ///     A transition node.
        /// </summary>
        private class TransitionNode : ITransitionNode
        {

            private readonly TState state;
            private readonly TEvent myEvent;
            private IState<TState, TEvent, TContext, TMessage> stateObject;

            public TransitionNode(
                TEvent myEvent,
                TState state)
            {
                this.myEvent = myEvent;
                this.state = state;
            }

            public async Task<TransitionResult<TEvent>> Run(
                Dictionary<TState, IState<TState, TEvent, TContext, TMessage>> stateLookup,
                IState<TState, TEvent, TContext, TMessage> currentState,
                TContext context,
                TMessage message)
            {
                if (Volatile.Read(ref this.stateObject) == null)
                {
                    if (stateLookup.ContainsKey(this.state))
                    {
                        Interlocked.CompareExchange(ref this.stateObject, stateLookup[this.state], null);
                    }
                    else
                    {
                        throw new UnableToFindStateException<TState>(this.state);
                    }
                }

                await currentState.Exit(
                    this.myEvent,
                    context,
                    message);
                return await Volatile.Read(ref this.stateObject).Entry(
                    this.myEvent,
                    context,
                    message);
            }
        }

        /// <summary>
        ///     An explicit ignore node.
        /// </summary>
        private class TransitionIgnoreNode : ITransitionNode
        {
            public Task<TransitionResult<TEvent>> Run(
                Dictionary<TState, IState<TState, TEvent, TContext, TMessage>> stateLookup,
                IState<TState, TEvent, TContext, TMessage> currentState,
                TContext context,
                TMessage message)
            {
                return Task.FromResult(TransitionResult<TEvent>.Stop());
            }
        }

        private ITransitionNode From(ValidTransition<TState, TEvent> transInfo)
        {
            switch (transInfo.ValidTransitionType)
            {
                case ValidTransitionType.Transition:
                    return new TransitionNode(
                        transInfo.Event,
                        transInfo.ToState);
                
                case ValidTransitionType.Ignore:
                    return new TransitionIgnoreNode();
                
                default:
                    throw new Exception($"Unknown valid transition past of {transInfo.ValidTransitionType}.");
            }
        }
    }
}