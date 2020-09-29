using System.Collections.Generic;
using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase
{
    public interface IStatePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>
        where TState : struct
        where TEvent : struct
        where TCtx : AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>

    {
        TState State { get; }
        
        IEnumerable<EventNodePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId>> Events { get; }

        Task Entry(TEvent @event, TUserId userId, TCtx ctx, TParam param);

        Task Exit(TEvent @event, TUserId userId, TCtx ctx, TParam param);
    }
}