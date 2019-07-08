using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mrh.Concurrent.StateMachine
{
    public interface IState<TState, TEvent, TCtx, TParam>
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateCtx<TState, TEvent, TParam>
    {

        TState State { get; }
        
        IEnumerable<EventNode<TState, TEvent, TCtx, TParam>> Events { get; }

        Task Entry(TEvent @event, TCtx ctx, TParam param);

        Task Exit(TEvent @event, TCtx ctx, TParam param);
    }
}