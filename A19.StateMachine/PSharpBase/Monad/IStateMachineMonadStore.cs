using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase.Monad
{
    public interface IStateMachineMonadStore<TKey, TState, TEvent, TParam, TCtx, TUserId>
        where TState: struct
        where TEvent: struct
        where TCtx : AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {

        /// <summary>
        ///     Used to get the state monad.
        /// </summary>
        /// <param name="key">The key for the state monad.</param>
        /// <param name="value">The value to be int he state monad.</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns>The state monad.</returns>
        Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue>> To<TValue>(TKey key, TValue value);
    }
}