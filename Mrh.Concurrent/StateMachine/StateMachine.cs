using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;

namespace Mrh.Concurrent.StateMachine
{
    public class StateMachine<TState, TEvent, TCtx, TParam> 
        where TState: struct 
        where TEvent: struct
        where TCtx: AbstractStateCtx<TState, TEvent, TParam>
    {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        
        private readonly Dictionary<TState, StateNode> states = new Dictionary<TState, StateNode>(10);

        /// <summary>
        ///     Used to register the context with the state machine.
        /// </summary>
        /// <param name="ctx">The context to register.</param>
        public void RegisterCtx(
            TCtx ctx)
        {
            ctx.NewEvent.Subscribe((x) =>
            {
                Task.Run(async () =>
                {
                    await this.HandleNextEvent(ctx);
                });
            });
        }

        /// <summary>
        ///     Triggers an event on the state machine.
        /// </summary>
        /// <param name="ctx">The state machine context.</param>
        /// <param name="event">The event to fire.</param>
        /// <param name="param">The parameter to the state machine.</param>
        /// <returns></returns>
        public bool Transition(
            TCtx ctx,
            TEvent @event,
            TParam param)
        {
            return ctx.Add(@event, param);
        }

        /// <summary>
        ///     Used to add a state to the state machine.
        /// </summary>
        /// <param name="state">The state to be registered. </param>
        /// <returns>The state machine.</returns>
        public StateMachine<TState, TEvent, TCtx, TParam> Add(
            IState<TState, TEvent, TCtx, TParam> state)
        {
            if (this.states.ContainsKey(state.State))
            {
                throw new StateAlreadyRegisteredException<TState>(state.State);
            }

            this.states[state.State] = new StateNode(state);
            return this;
        }

        private async Task HandleNextEvent(TCtx ctx)
        {
            if (ctx.Next(out var @event))
            {
                if (!this.states.TryGetValue(ctx.CurrentState, out var stateNode))
                {
                    throw new UnableToFindStateException<TState>(ctx.CurrentState);
                }
                if (stateNode.TryGetNode(@event.Event, out var eventAction))
                {
                    switch (eventAction.EventAction)
                    {
                        case EventActionType.Defer:
                            throw new ArgumentException("Deferred currently not supported.");
                        
                        case EventActionType.Do:
                            await eventAction.Action.Execute(
                                @event.Event,
                                ctx,
                                @event.Param);
                            break;
                        
                        case EventActionType.Goto:
                            await stateNode.State.Exit(
                                @event.Event,
                                ctx,
                                @event.Param);
                            ctx.CurrentState = eventAction.State.Value;
                            if (this.states.TryGetValue(ctx.CurrentState, out var node))
                            {
                                await node.State.Entry(
                                    @event.Event,
                                    ctx,
                                    @event.Param);
                            }
                            break;
                        
                        case EventActionType.Ignore:
                            break;
                    }
                }
            }
        }

        private class StateNode
        {
            private readonly Dictionary<TEvent, EventNode<TState, TEvent, TCtx, TParam>> eventActLookup = new Dictionary<TEvent, EventNode<TState, TEvent, TCtx, TParam>>(10);

            public readonly IState<TState, TEvent, TCtx, TParam> State;

            public StateNode(
                IState<TState, TEvent, TCtx, TParam> state)
            {
                this.State = state;
                foreach (var @event in state.Events)
                {
                    if (this.eventActLookup.ContainsKey(@event.Event))
                    {
                        throw new Exception($"Duplicate event registered {@event.Event}.");
                    }
                    this.eventActLookup[@event.Event] = @event;
                }
            }

            public bool TryGetNode(
                TEvent @event,
                out EventNode<TState, TEvent, TCtx, TParam> eventNode)
            {
                return this.eventActLookup.TryGetValue(
                    @event,
                    out eventNode);
            }
        }
    }
}