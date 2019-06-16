using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mrh.StateMachine.Test
{
    public class InMemoryTransitionStore<TState, TEvent, TContext, TMessage> : ITransitionStore<TState, TEvent, TContext, TMessage>
        where TState: struct
        where TEvent: struct
        where TContext: IEventContext<TState>
    {
        
        public readonly List<SavedNode> Transitions = new List<SavedNode>(10);
        
        public Task Save(TransitionResultType result, TState state, TEvent? myEvent, TContext context, TMessage message)
        {
            this.Transitions.Add(new SavedNode(
                result,
                state,
                myEvent,
                message));
            return Task.FromResult(0);
        }

        public Task Save(TMessage message)
        {
            return Task.FromResult(0);
        }

        public struct SavedNode
        {
            public readonly TransitionResultType ResultType;
            public readonly TState State;
            public readonly TEvent? Event;
            public readonly TMessage Message;

            public SavedNode(
                TransitionResultType resultType,
                TState state,
                TEvent? @event,
                TMessage message)
            {
                this.ResultType = resultType;
                this.State = state;
                this.Event = @event;
                this.Message = message;
            }
        }
    }
}