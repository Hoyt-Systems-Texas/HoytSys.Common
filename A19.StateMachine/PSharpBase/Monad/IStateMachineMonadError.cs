using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase.Monad
{
    public interface IStateMachineMonadError<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue> :
        
        IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue>
        where TState : struct
        where TEvent : struct
        where TCtx : AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="TR">The result type.</typeparam>
        /// <returns></returns>
        Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TR>> To<TR>();
    }
}