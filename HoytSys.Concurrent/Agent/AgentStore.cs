using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HoytSys.Core;
using NLog;

namespace A19.Concurrent.Agent
{
    public class AgentStore<TContext> : IAgentStore<TContext>, IStartable, IStoppable where TContext : IAgentContext
    {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<long, AgentLocalRunner<TContext>> runners;

        private readonly IAgentContextStore<TContext> agentContextStore;

        private readonly uint queueSize;

        private readonly TimeSpan cleanInterval;

        private Timer cleaner;

        private readonly long timeOutAfterFreq;

        private readonly StopWatchThreadSafe cleanStopWatch = new StopWatchThreadSafe();

        private readonly long cleanInFrequency;

        private int cleanState = (int) AgentCleanState.Inactive;

        public AgentStore(
            uint queueSize,
            TimeSpan timeout,
            TimeSpan cleanInterval,
            IAgentContextStore<TContext> agentContextStore)
        {
            this.queueSize = queueSize;
            this.cleanInterval = cleanInterval;
            this.runners = new ConcurrentDictionary<long, AgentLocalRunner<TContext>>(
                5,
                1000);
            this.agentContextStore = agentContextStore;
            this.timeOutAfterFreq = StopWatchThreadSafe.MillsToFrequency((long) timeout.TotalMilliseconds);
            this.cleanInFrequency = StopWatchThreadSafe.MillsToFrequency((int) cleanInterval.TotalMilliseconds);
        }

        public async Task<IAgentMonad<TContext, T>> To<T>(long id, T value)
        {
            if (this.runners.TryGetValue(id, out AgentLocalRunner<TContext> runner))
            {
                return runner.To(value);
            }
            
            if (this.CheckClean())
            {
                this.ChangeState(AgentCleanState.Running);
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

        private bool CheckClean()
        {
            return (this.cleanStopWatch.Elapsed() > this.cleanInFrequency);
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

        private void Clear()
        {
            try
            {
                var remove = new List<AgentLocalRunner<TContext>>(100);
                foreach (var run in this.runners.Values)
                {
                    if (run.IsExpired(this.timeOutAfterFreq))
                    {
                        remove.Add(run);
                    }
                }

                foreach (var runner in remove)
                {
                    if (runner.Remove())
                    {
                        this.runners.TryRemove(runner.Id, out AgentLocalRunner<TContext> _);
                        runner.Removed();
                    }
                }
                this.ChangeState(AgentCleanState.Stopping);
            }
            catch (Exception ex)
            {
                log.Error(ex, ex.Message);
                this.ChangeState(AgentCleanState.Error);
            }
        }

        private void ChangeState(AgentCleanState newState)
        {
            switch (newState)
            {
                case AgentCleanState.Inactive:
                    if (this.SetState(AgentCleanState.Stopping, AgentCleanState.Inactive))
                    {
                        this.cleanStopWatch.Reset();
                    }
                    break;
                
                case AgentCleanState.Running:
                    if (this.SetState(AgentCleanState.Inactive, AgentCleanState.Running))
                    {
                        this.Clear();
                    }
                    break;
                
                case AgentCleanState.Stopping:
                    this.SetState(AgentCleanState.Running, AgentCleanState.Stopping);
                    this.ChangeState(AgentCleanState.Inactive);
                    break;
                
                case AgentCleanState.Error:
                    this.SetState((AgentCleanState) this.cleanState, AgentCleanState.Stopping);
                    this.ChangeState(AgentCleanState.Inactive);
                    break;
            }
        }

        private bool SetState(AgentCleanState currentState, AgentCleanState newState)
        {
            return Interlocked.CompareExchange(
                       ref this.cleanState,
                       (int) newState,
                       (int) currentState) == (int) currentState;
        }

        private enum AgentCleanState
        {
            Inactive = 1,
            Running = 2,
            Stopping = 3,
            Error = 4
        }
    }
}