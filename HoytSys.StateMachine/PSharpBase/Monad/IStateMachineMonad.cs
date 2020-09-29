using System;
using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase.Monad
{
    public interface IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue>
        where TState : struct
        where TEvent : struct
        where TCtx : AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>

    {
        /// <summary>
        ///     Used to run an action on the state machine thread.
        /// </summary>
        /// <param name="func">The function to run.</param>
        /// <typeparam name="TR">The result type.</typeparam>
        /// <returns>The state machine monad.</returns>
        Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TR>> Run<TR>(
            Func<TCtx, TValue, Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TR>>> func);

        /// <summary>
        ///     Used to run an event on the state machine.
        /// </summary>
        /// <param name="executedBy">The user who triggered the event.</param>
        /// <param name="event">The event to run on the state machine.</param>
        /// <param name="param">The parameter to pass to the state machine.</param>
        /// <returns>The state machine monad result.</returns>
        Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TState>> Event(
            TUserId executedBy,
            TEvent @event,
            TParam param);
    }
}