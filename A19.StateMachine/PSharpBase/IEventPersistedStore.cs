using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase
{
    public interface IEventPersistedStore<TKey, TEvent, TParam, TUserId>
        where TEvent: struct
    {
        /// <summary>
        ///     Used to save the persisted information to the database and returns the key.
        /// </summary>
        /// <returns>The state that saves the info to the database.</returns>
        Task<EventActionNodePersist<TKey, TEvent, TParam, TUserId>> Save(
            EventActionNodePersist<TKey, TEvent, TParam, TUserId> eventNodePersisted);

        /// <summary>
        ///     Used to save the result of applying the action to the data store.
        /// </summary>
        /// <param name="eventKey">The event key.</param>
        /// <param name="resultType">The result type of applying that action.</param>
        /// <returns>The task that does the update.</returns>
        Task SaveResult(long eventKey, EventResultType resultType);

    }
}