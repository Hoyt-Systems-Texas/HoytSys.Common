using System;
using System.Threading.Tasks;
using A19.Messaging.Common;
using Mrh.Monad;

namespace A19.StateMachine.PSharpBase.Monad
{
    public static class StateMachineMonadExt
    {
        
        /// <summary>
        ///     Used to convert a state monad to a result monad.
        /// </summary>
        /// <param name="monadT"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TParam"></typeparam>
        /// <typeparam name="TCtx"></typeparam>
        /// <typeparam name="TUserId"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Unable to convert the state monad to the result monad.</exception>
        public static async Task<IResultMonad<TResult>> ToResult<TKey, TState, TEvent, TParam, TCtx, TUserId, TResult>(
            Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TResult>> monadT)
            where TState: struct
            where TEvent: struct
            where TCtx: AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
        {
            var monad = await monadT;
            if (monad is StateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TResult> s)
            {
                return new ResultSuccess<TResult>(s.Value);
            } else if (monad is StateMachineFullMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TResult> f)
            {
                return new ResultMonadBusy<TResult>();
            }
            else
            {
                throw new ArgumentException($"Invalid state monad type passed {monad.GetType()}", nameof(monad));
            }
        }
            
    }
}