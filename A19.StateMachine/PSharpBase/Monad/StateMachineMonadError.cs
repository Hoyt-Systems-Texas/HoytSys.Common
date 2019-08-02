using System;
using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase.Monad
{
    public abstract class StateMachineMonadError<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue>
        : IStateMachineMonadError<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue>
        where TState : struct
        where TEvent : struct
        where TCtx : AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        public Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TR>> Run<TR>(Func<TCtx, TValue, Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TR>>> func)
        {
            return this.To<TR>();
        }

        public Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TState>> Event(TUserId executedBy, TEvent @event, TParam param)
        {
            return this.To<TState>();
        }

        public abstract Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TR>> To<TR>();
    }
}