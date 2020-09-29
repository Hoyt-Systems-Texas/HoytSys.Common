namespace A19.StateMachine.PSharpBase
{
    /// <summary>
    ///     Used to get a context from the data store.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TParam"></typeparam>
    /// <typeparam name="TCtx"></typeparam>
    /// <typeparam name="TUserId"></typeparam>
    public interface IStateMachineCtxStore<TKey, TState, TEvent, TParam, TCtx, TUserId> 
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {

        TCtx GetContext(TKey key);
    }
}