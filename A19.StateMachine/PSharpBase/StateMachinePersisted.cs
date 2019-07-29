using System;
using System.Collections.Generic;
using A19.Concurrent.StateMachine;
using NLog;

namespace A19.StateMachine.PSharpBase
{
    public class StateMachinePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>
        where TState : struct
        where TEvent : struct
        where TCtx: AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TUserId>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly Action<Exception> errorHandler;
        
        private class StateNode
        {
            private readonly Dictionary<TEvent, EventNodePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>> eventActLookup = 
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