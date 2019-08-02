using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase.Monad
{
    public sealed class StateMachineFullMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue>
        : StateMachineMonadError<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue>
        where TState : struct
        where TEvent : struct
        where TCtx : AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        public override Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TR>> To<TR>()
        {
            return Task.FromResult(
                (IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TR>)
                new StateMachineFullMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TR>());
        }
    }
}