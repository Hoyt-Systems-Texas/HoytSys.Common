using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase.Monad
{
    
    /// <summary>
    ///     The type when a monad is full.
    /// </summary>
    /// <typeparam name="TKey">The key type for the state machine.</typeparam>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <typeparam name="TParam">The type for the parameter.</typeparam>
    /// <typeparam name="TCtx">The context type.</typeparam>
    /// <typeparam name="TUserId">The type for the user.</typeparam>
    /// <typeparam name="TValue">The value being passed around.</typeparam>
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