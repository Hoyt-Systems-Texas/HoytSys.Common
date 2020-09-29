using System;
using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase
{
    public interface IExecuteActionFunc<TKey, TState, TEvent, TParam, TCtx, TUserId>
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        Task Run(TCtx ctx);

        void SetError(Exception ex);
    }
}