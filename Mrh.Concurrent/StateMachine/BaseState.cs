using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mrh.Concurrent.StateMachine
{
    public abstract class BaseState<TState, TEvent, TCtx, TParam> : IState<
        TState,
        TEvent,
        TCtx,
        TParam>
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateCtx<TState, TEvent, TParam>
    {
        public abstract TState State { get; }
        public abstract IEnumerable<EventNode<TState, TEvent, TCtx, TParam>> Events { get; }

        public virtual Task Entry(TEvent @event, TCtx ctx, TParam param)
        {
            return Task.FromResult(0);
        }

        public virtual Task Exit(TEvent @event, TCtx ctx, TParam param)
        {
            return Task.FromResult(0);
        }
    }
}