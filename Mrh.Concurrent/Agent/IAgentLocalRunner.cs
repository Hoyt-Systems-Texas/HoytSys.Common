using System;
using System.Threading.Tasks;

namespace Mrh.Concurrent.Agent
{
    public interface IAgentLocalRunner<TContext> where TContext : IAgentContext
    {
        Task<IAgentMonad<TContext, TR>> Run<T, TR>(
            IAgentMonad<TContext, T> monad,
            Func<TContext, T, Task<IAgentMonad<TContext, TR>>> func);

        IAgentMonad<TContext, T> To<T>(T value);
    }
}