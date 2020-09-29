using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase
{
    public interface IStateMachinePersistedCtxStore<TKey, TState, TEvent, TParam, TCtx, TUserId>
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        /// <summary>
        ///     Used to get the state machine context out of the data store.
        /// </summary>
        /// <param name="key">The key of the state machine to load.</param>
        /// <returns>The state machine ctx.</returns>
        Task<TCtx> Load(TKey key);
    }
}