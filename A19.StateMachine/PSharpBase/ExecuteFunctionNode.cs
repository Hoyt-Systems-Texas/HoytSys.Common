using System;
using System.Threading.Tasks;
using Mrh.Monad;

namespace A19.StateMachine.PSharpBase
{
    public class ExecuteFunctionNode<TKey, TState, TEvent, TParam, TCtx, TUserId, TResult> : IExecuteActionFunc<TKey, TState, TEvent, TParam, TCtx, TUserId>
        where TState : struct
        where TEvent : struct
        where TCtx : AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        public readonly Func<TCtx, Task<IResultMonad<TResult>>> Func;

        public readonly TaskCompletionSource<IResultMonad<TResult>> TaskCompletionSource =
            new TaskCompletionSource<IResultMonad<TResult>>(TaskCreationOptions.RunContinuationsAsynchronously);

        public ExecuteFunctionNode(Func<TCtx, Task<IResultMonad<TResult>>> func)
        {
            this.Func = func;
        }

        public async Task Run(TCtx ctx)
        {
            var result = await this.Func(ctx);
            this.TaskCompletionSource.SetResult(result);
        }

        public void SetError(Exception ex)
        {
            this.TaskCompletionSource.SetException(ex);
        }
    }
}