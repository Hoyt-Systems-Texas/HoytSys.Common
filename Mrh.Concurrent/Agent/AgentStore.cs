using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Mrh.Core;

namespace Mrh.Concurrent.Agent
{
    public class AgentStore<TContext> : IAgentStore<TContext>, IStartable, IStoppable where TContext: IAgentContext
    {

        private readonly ConcurrentDictionary<long, AgentLocalRunner<TContext>> runners;

        private readonly IAgentContextStore<TContext> agentContextStore;

        private readonly uint queueSize;

        private readonly TimeSpan cleanInterval;
        
        private Timer cleaner;

        public AgentStore(
            uint queueSize,
            TimeSpan cleanInterval,
            IAgentContextStore<TContext> agentContextStore)
        {
            this.queueSize = queueSize;
            this.cleanInterval = cleanInterval;
            this.runners = new ConcurrentDictionary<long, AgentLocalRunner<TContext>>(
                5,
                1000);
            this.agentContextStore = agentContextStore;
        }
        
        public async Task<IAgentMonad<TContext, T>> To<T>(long id, T value)
        {
            if (this.runners.TryGetValue(id, out AgentLocalRunner<TContext> runner))
            {
                return runner.To(value);
            }

            while (true)
            {
                var context = await this.agentContextStore.Lookup(id);
                runner = new AgentLocalRunner<TContext>(id, queueSize, context);
                if (this.runners.TryAdd(id, runner))
                {
                    return runner.To(value);
                }

                if (this.runners.TryGetValue(id, out runner))
                {
                    return runner.To(value);
                }
            }
        }

        public Task<IAgentMonad<TContext, int>> To(long id)
        {
            return this.To(id, 0);
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}