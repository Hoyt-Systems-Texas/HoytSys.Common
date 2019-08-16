using System.Collections.Concurrent;
using System.Collections.Generic;
using A19.Concurrent;

namespace A19.StateMachine.PSharpBase.Monad
{
    public class StateMachineMonadStore<TKey, TState, TEvent, TParam, TCtx, TUserId>
        : IStateMachineMonadStore<TKey, TState, TEvent, TParam, TCtx, TUserId>
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        
        private readonly ConcurrentDictionary<TKey, ContextNode> stateMachineCtx = new ConcurrentDictionary<TKey, ContextNode>();
        
        public IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue> To<TValue>(TKey key, TValue value)
        {
            throw new System.NotImplementedException();
        }

        private struct ContextNode
        {
            public readonly StopWatchThreadSafe LastUsed;
            public readonly TCtx context;

            public ContextNode(TCtx context)
            {
                this.LastUsed = new StopWatchThreadSafe();
                this.context = context;
            }
        }
    }
}