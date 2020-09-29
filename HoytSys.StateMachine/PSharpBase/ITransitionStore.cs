using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase
{
    public interface ITransitionStore<TKey, TState, TUserId>
        where TState: struct
    {
        /// <summary>
        /// </summary>
        /// <param name="key">The type for the key.</param>
        /// <param name="newState">The new state of the state machine.</param>
        /// <param name="userId">The id of the user who caused the change to be made.</param>
        /// <returns>The id of the generated state.</returns>
        Task<long> SaveTransition(TKey key, TState newState, TUserId userId);
    }
}