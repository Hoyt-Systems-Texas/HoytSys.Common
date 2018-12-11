using System;
using System.Threading.Tasks;

namespace Mrh.Concurrent.Agent
{

    public interface IAgentMonad<TContext, out T> where TContext: IAgentContext
    {
        /// <summary>
        ///     Simple implementation of the bind method.
        /// </summary>
        /// <param name="func">The function to run.</param>
        /// <typeparam name="TR">The type we are returning.</typeparam>
        /// <returns>The binding action on the monad.</returns>
        Task<IAgentMonad<TContext, TR>> Bind<TR>(Func<TContext, T, Task<IAgentMonad<TContext, TR>>> func);

        Task<IAgentMonad<TContext, TR>> Select<TR>(Func<TContext, T, Task<TR>> func);
    }

    public interface IAgentFailure<TContext, T> : IAgentMonad<TContext, T> where TContext : IAgentContext
    {
        Task<IAgentMonad<TContext, TR>> To<TR>();
    } 

    /// <summary>
    ///     A class to represent when an agent is to busy to process ongoing requests.
    /// </summary>
    /// <typeparam name="TContext">The context</typeparam>
    /// <typeparam name="T">The type.</typeparam>
    public class AgentBusy<TContext, T> : IAgentFailure<TContext, T> where TContext : IAgentContext
    {
        public Task<IAgentMonad<TContext, TR>> Bind<TR>(Func<TContext, T, Task<IAgentMonad<TContext, TR>>> func)
        {
            return this.To<TR>();
        }

        public Task<IAgentMonad<TContext, TR>> Select<TR>(Func<TContext, T, Task<TR>> func)
        {
            return this.To<TR>();
        }

        public Task<IAgentMonad<TContext, TR>> To<TR>()
        {
            return Task.FromResult<IAgentMonad<TContext, TR>>(new AgentBusy<TContext, TR>());
        }
    }

    public class AgentRemoved<TContext, T> : IAgentFailure<TContext, T> where TContext : IAgentContext
    {
        public Task<IAgentMonad<TContext, TR>> Bind<TR>(Func<TContext, T, Task<IAgentMonad<TContext, TR>>> func)
        {
            return this.To<TR>();
        }

        public Task<IAgentMonad<TContext, TR>> Select<TR>(Func<TContext, T, Task<TR>> func)
        {
            return this.To<TR>();
        }

        public Task<IAgentMonad<TContext, TR>> To<TR>()
        {
            return Task.FromResult<IAgentMonad<TContext, TR>>(new AgentBusy<TContext, TR>());
        }
    }
    
    public class AgentMonad<TContext, T> : IAgentMonad<TContext, T> where TContext: IAgentContext
    {
        public readonly T Value;

        private readonly IAgentLocalRunner<TContext> runner;

        public AgentMonad(
            T value,
            IAgentLocalRunner<TContext> runner)
        {
            this.Value = value;
            this.runner = runner;
        }

        public Task<IAgentMonad<TContext, TR>> Bind<TR>(Func<TContext, T, Task<IAgentMonad<TContext, TR>>> func)
        {
            return this.runner.Run(this, func);
        }

        public Task<IAgentMonad<TContext, TR>> Select<TR>(Func<TContext, T, Task<TR>> func)
        {
            return this.runner.Run(this, 
                async (context, value) => 
                    this.To(await func(context, value)));
        }

        public IAgentMonad<TContext, TR> To<TR>(TR state)
        {
            return new AgentMonad<TContext, TR>(
                state,
                this.runner);
        }
    }
}