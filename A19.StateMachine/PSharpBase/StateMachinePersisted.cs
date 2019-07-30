using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using A19.Concurrent.StateMachine;
using NLog;

namespace A19.StateMachine.PSharpBase
{
    public class StateMachinePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>
        where TState : struct
        where TEvent : struct
        where TCtx : AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly Action<Exception> errorHandler;
        private readonly Dictionary<TState, StateNode> states = new Dictionary<TState, StateNode>(100);
        private readonly ITransitionStore<TKey, TState, TUserId> transitionStore;
        private readonly IEventPersistedStore<TKey, TState, TEvent, TParam, TCtx, TUserId> eventPersistedStore;

        public StateMachinePersisted(
            ITransitionStore<TKey, TState, TUserId> transitionStore,
            IEventPersistedStore<TKey, TState, TEvent, TParam, TCtx, TUserId> eventPersistedStore)
        {
            this.transitionStore = transitionStore;
            this.eventPersistedStore = eventPersistedStore;
        }

        public StateMachinePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId> Add(
            IStatePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId> state)
        {
            if (!this.states.ContainsKey(state.State))
            {
                var node = new StateNode(state);
                this.states[state.State] = new StateNode(state);
            }
            else
            {
                throw new DuplicateStateRegisterException<TState>(state.State);
            }

            return this;
        }

        public async Task HandleTransition(TCtx ctx)
        {
            while (ctx.Next(out var node))
            {
                if (node.RunOnThread)
                {
                    try
                    {
                        await node.Func.Run(ctx);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, ex.Message);
                        node.Func.SetError(ex);
                    }
                    finally
                    {
                        await ctx.DoneWithEvent();
                    }
                }
                else
                {
                    try
                    {
                        if (this.states.TryGetValue(ctx.CurrentState, out var stateNode))
                        {
                            if (stateNode.TryGetNode(node.Event, out var eventNode))
                            {
                                switch (eventNode.EventAction)
                                {
                                    case EventActionType.Defer:
                                        ctx.Skip(node);
                                        continue; // Skip everything.

                                    case EventActionType.Do:
                                        await eventNode.Action.Run(
                                            node.Event,
                                            node.CreatedBy,
                                            ctx,
                                            node.Param);
                                        break;

                                    case EventActionType.Goto:
                                        if (this.states.TryGetValue(eventNode.State.Value, out var newNode))
                                        {
                                            if (node.EventResult != EventResultType.ExitCompleted)
                                            {
                                                await stateNode.State.Exit(
                                                    node.Event,
                                                    node.CreatedBy,
                                                    ctx,
                                                    node.Param);
                                                ctx.CurrentState = eventNode.State.Value;
                                                await this.eventPersistedStore.SaveResult(node.Id,
                                                    EventResultType.ExitCompleted);
                                            }

                                            try
                                            {
                                                await newNode.State.Entry(
                                                    node.Event,
                                                    node.CreatedBy,
                                                    ctx,
                                                    node.Param);
                                                ctx.ResetSkip();
                                            }
                                            catch (Exception ex)
                                            {
                                                log.Error(ex, ex.Message);
                                            }
                                        }

                                        break;

                                    case EventActionType.Ignore:
                                        break;
                                }
                            }
                        }
                        else
                        {
                            log.Warn($"Unable to find state {ctx.CurrentState}");
                        }

                        await ctx.DoneWithEvent();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, ex.Message);
                        ctx.RetryEvent(node);
                    }
                }
            }
        }

        private class StateNode
        {
            private readonly Dictionary<TEvent, EventNodePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>>
                eventActLookup =
                    new Dictionary<TEvent, EventNodePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>>(10);

            public readonly IStatePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId> State;

            public StateNode(
                IStatePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId> state)
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
                out EventNodePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId> eventNode)
            {
                return this.eventActLookup.TryGetValue(
                    @event,
                    out eventNode);
            }
        }
    }
}