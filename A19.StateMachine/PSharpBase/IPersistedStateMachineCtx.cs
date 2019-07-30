namespace A19.StateMachine.PSharpBase
{
    /// <summary>
    ///      Get the context.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TParam"></typeparam>
    /// <typeparam name="TCtx"></typeparam>
    /// <typeparam name="TUserId">The type for the user.</typeparam>
    public interface IPersistedStateMachineCtx<TKey, TState, TEvent, TParam, TCtx, TUserId> 
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        /// <summary>
        ///     Used to get the context by the key.
        /// </summary>
        /// <param name="key">The key for the state machine to get.</param>
        /// <returns>The state machine context with the specified key.</returns>
        TCtx GetCtx(TKey key);
    }
}