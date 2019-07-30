using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase
{
    public interface IAction<TKey, TState, TEvent, TCtx, TParam, TUserId>
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        /// <summary>
        ///     Called when to execute the action.
        /// </summary>
        /// <param name="event">The event that trigger the action.</param>
        /// <param name="userId">The id of the user who made the change.</param>
        /// <param name="ctx">The state machine ctx.</param>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        Task Run(TEvent @event, TUserId userId, TCtx ctx, TParam param);
    }
}