using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Mrh.Concurrent.Agent
{
    public class AgentLocalRunner<TContext> : IAgentLocalRunner<TContext> where TContext: IAgentContext
    {
        
        private const int STOPPED = 1;
        private const int PENDING_RUNNING = 2;
        private const int RUNNING = 3;
        private const int REMOVING = 4;
        private const int REMOVED = 5;
        
        public readonly long Id;

        private int currentState = STOPPED;
        
        private readonly StopWatchThreadSafe lastRan = new StopWatchThreadSafe();

        private readonly MpmcRingBuffer<IAgentAsyncNode> queue;

        private TContext context;

        public AgentLocalRunner(
            long id,
            uint queueSize,
            TContext context)
        {
            this.Id = id;
            this.queue = new MpmcRingBuffer<IAgentAsyncNode>(queueSize);
            this.context = context;
        }

        public Task<IAgentMonad<TContext, TR>> Run<T, TR>(
            IAgentMonad<TContext, T> monad,
            Func<TContext, T, Task<IAgentMonad<TContext, TR>>> func)
        {
            var stopWatch = new Stopwatch();
            if (monad is AgentMonad<TContext, T> agentMonad)
            {
                var node = new AgentAsyncNode<T, TR>(func, agentMonad);
                if (this.queue.Offer(node))
                {
                    this.ChangeState(PENDING_RUNNING);
                    return node.Task.Task;
                }
                else
                {
                    return Task.FromResult((IAgentMonad<TContext, TR>) new AgentBusy<TContext, TR>());
                }
            } else if (monad is IAgentFailure<TContext, T> failure)
            {
                return failure.To<TR>();
            }
            else
            {
                throw new Exception($"Do not know how to handle type {monad.GetType()}");
            }
        }

        public IAgentMonad<TContext, T> To<T>(T value)
        {
            return new AgentMonad<TContext, T>(
                value,
                this);
        }
        
        protected int CurrentState
        {
            get { return Volatile.Read(ref this.currentState); }
        }

        protected bool CasChangeState(int newState, int expectedState)
        {
            return Interlocked.CompareExchange(
                ref this.currentState,
                newState,
                expectedState) == expectedState;
        }

        protected void SetState(int newState)
        {
            Volatile.Write(ref this.currentState, newState);
        }

        protected void ChangeState(int newState)
        {
            var currentState = CurrentState;
            switch (newState)
            {
                case STOPPED:
                    // Have to use a case here just in case we are caught while trying to remove the node.
                    if (this.CasChangeState(STOPPED, currentState)) {
                        if (this.queue.TryPeek(out IAgentAsyncNode ignore))
                        {
                            this.SetState(PENDING_RUNNING);
                            this.ChangeState(RUNNING);
                        }
                        else
                        {
                            this.SetState(STOPPED);
                        }
                    }
                    return;

                case PENDING_RUNNING:
                    if (this.CasChangeState(PENDING_RUNNING, STOPPED))
                    {
                        this.ChangeState(RUNNING);
                    }

                    return;
                
                case RUNNING:
                    // The guard here is PENDING_RUNNING we can safely set the state using setState.
                    this.lastRan.Reset();
                    this.SetState(RUNNING);
                    this.queue.Drain(val => { val.Run(this.context); }, 10);
                    this.ChangeState(STOPPED);
                    return;
                
                case REMOVED:
                    if (this.CasChangeState(REMOVED, REMOVING))
                    {
                        // Mark all pending requests as removed.
                        while (this.queue.TryPoll(out IAgentAsyncNode value))
                        {
                            value.MarkRemoved();
                        }
                    }
                    return;
            }
            if (currentState == REMOVED)
            {
                while (this.queue.TryPoll(out IAgentAsyncNode value))
                {
                    value.MarkRemoved();
                }
            }
        }

        public bool IsExpired(long time)
        {
            return this.lastRan.Elapsed() > time;
        }

        /// <summary>
        ///     Mark the node for removal.
        /// </summary>
        /// <returns>true if the node was successfully marked for removal.</returns>
        public bool Remove()
        {
            return this.CasChangeState(REMOVING, STOPPED);
        }

        /// <summary>
        ///     Handles removing the remaining values from the node.
        /// </summary>
        public void Removed()
        {
            this.ChangeState(REMOVED);
        }

        private interface IAgentAsyncNode
        {
            void Run(TContext context);

            void MarkRemoved();
        }
        
        private sealed class AgentAsyncNode<T, TR> : IAgentAsyncNode
        {
            public readonly Func<TContext, T, Task<IAgentMonad<TContext, TR>>> Func;

            public readonly TaskCompletionSource<IAgentMonad<TContext, TR>> Task;

            public readonly AgentMonad<TContext, T> Monad;
            
            public AgentAsyncNode(
                Func<TContext, T, Task<IAgentMonad<TContext, TR>>> func,
                AgentMonad<TContext, T> monad)
            {
                this.Monad = monad;
                this.Func = func;
                this.Task = new TaskCompletionSource<IAgentMonad<TContext, TR>>();
            }

            public void Run(TContext context)
            {
                try
                {
                    var task = this.Func(context, Monad.Value);
                    task.Wait();
                    this.Task.SetResult(task.Result);
                }
                catch (Exception ex)
                {
                    this.Task.SetException(ex);
                }
            }

            public void MarkRemoved()
            {
                this.Task.SetResult(new AgentRemoved<TContext, TR>());
            }
        }
    }
}