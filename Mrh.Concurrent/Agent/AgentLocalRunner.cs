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
        
        private readonly long id;

        private int currentState = STOPPED;
        
        private readonly StopWatchThreadSafe lastRan = new StopWatchThreadSafe();

        private readonly MpmcRingBuffer<IAgentAsyncNode> queue;

        private TContext context;

        public AgentLocalRunner(
            long id,
            uint queueSize,
            TContext context)
        {
            this.id = id;
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
            switch (newState)
            {
                case STOPPED:
                    if (this.queue.TryPeek(out IAgentAsyncNode ignore))
                    {
                        this.SetState(PENDING_RUNNING);
                        this.ChangeState(RUNNING);
                    }
                    else
                    {
                        this.SetState(STOPPED);
                    }
                    break;

                case PENDING_RUNNING:
                    if (this.CasChangeState(PENDING_RUNNING, STOPPED))
                    {
                        this.ChangeState(RUNNING);
                    }
                    break;
                
                case RUNNING:
                    this.lastRan.Reset();
                    this.SetState(RUNNING);
                    this.queue.Drain(val => { val.Run(this.context); }, 10);
                    this.ChangeState(STOPPED);
                    break;
                
            }
        }

        private interface IAgentAsyncNode
        {
            void Run(TContext context);
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
        }
    }
}