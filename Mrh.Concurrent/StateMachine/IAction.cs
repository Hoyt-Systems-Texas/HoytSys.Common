using System.Threading.Tasks;

namespace Mrh.Concurrent.StateMachine
{
    public interface IAction<TState, TEvent, TCtx, TParam>
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateCtx<TState, TEvent, TParam>
    {
        Task Execute(TEvent @event, TCtx ctx, TParam param);
    }
}