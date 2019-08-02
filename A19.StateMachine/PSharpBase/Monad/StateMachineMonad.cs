using System;
using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase.Monad
{
    public sealed class StateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue> 
        : IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue>
        where TState : struct
        where TEvent : struct
        where TCtx : AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        public readonly TValue Value;

        private readonly TCtx ctx;

        public StateMachineMonad(
            TValue value,
            TCtx ctx)
        {
            this.Value = value;
            this.ctx = ctx;
        }

        public async Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TR>> Run<TR>(Func<TCtx, TValue, Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TR>>> func)
        {
            var executeNode = new ExecuteFunctionNode<TKey, TState, TEvent, TParam, TCtx, TUserId,
                IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TR>>(
                (ctx) => { return func(ctx, this.Value); });
            var result = await this.ctx.Add(new EventActionNodePersist<TKey, TState, TEvent, TParam, TCtx, TUserId>
            {
                RunOnThread = true,
                Func = executeNode
            });
            if (result)
            {
                return await executeNode.TaskCompletionSource.Task;
            }
            else
            {
                // TODO return an error.
                return null;
            }
        }

        public async Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TState>> Event(
            TUserId executedBy,
            TEvent @event,
            TParam param)
        {
            var result = await ctx.Add(new EventActionNodePersist<TKey, TState, TEvent, TParam, TCtx, TUserId>
            {
                CreatedBy = executedBy,
                Event = @event,
                Param = param
            });
            if (result)
            {
                return new StateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TState>(
                    ctx.CurrentState,
                    ctx);
            }
            else
            {
                // TODO Return an error.
                return null;
            }
        }
    }
}