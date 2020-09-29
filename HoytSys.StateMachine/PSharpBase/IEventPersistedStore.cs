using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase
{
    public interface IEventPersistedStore<TKey, TState, TEvent, TParam, TCtx, TUserId>
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        /// <summary>
        ///     Used to save the persisted information to the database and returns the key.
        /// </summary>
        /// <returns>The state that saves the info to the database.</returns>
        Task<EventActionNodePersist<TKey, TState, TEvent, TParam, TCtx, TUserId>> Save(
            EventActionNodePersist<TKey, TState, TEvent, TParam, TCtx, TUserId> eventNodePersisted);

        /// <summary>
        ///     Used to save the result of applying the action to the data store.
        /// </summary>
        /// <param name="eventKey">The event key.</param>
        /// <param name="resultType">The result type of applying that action.</param>
        /// <returns>The task that does the update.</returns>
        Task SaveResult(long eventKey, EventResultType resultType);

    }
}