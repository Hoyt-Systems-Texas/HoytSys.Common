using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using A19.Concurrent;
using NLog;

namespace A19.StateMachine.PSharpBase.Monad
{
    /// <summary>
    ///     The store for the context for a single service running.  Doesn't support multiple nodes!
    /// </summary>
    /// <typeparam name="TKey">The key for the context.</typeparam>
    /// <typeparam name="TState">The type for the state.</typeparam>
    /// <typeparam name="TEvent">The type for the event.</typeparam>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <typeparam name="TCtx">The context type.</typeparam>
    /// <typeparam name="TUserId">The type for the user id.</typeparam>
    public class StateMachineMonadStore<TKey, TState, TEvent, TParam, TCtx, TUserId>
        : IStateMachineMonadStore<TKey, TState, TEvent, TParam, TCtx, TUserId>
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        
        private const int CLEAN_IDLE = 0;
        private const int CLEAN_RUNNING = 1;
        private int cleanerRunningState = CLEAN_IDLE;
        
        private readonly ConcurrentDictionary<TKey, ContextNode> stateMachineCtx = new ConcurrentDictionary<TKey, ContextNode>();
        private readonly IStateMachinePersistedCtxStore<TKey, TState, TEvent, TParam, TCtx, TUserId>
            stateMachinePersistedCtxStore;
        private readonly long expiresAfter;
        private readonly long cleanAfter;
        private readonly StopWatchThreadSafe lastCleaned = new StopWatchThreadSafe();

        public StateMachineMonadStore(
            IStateMachinePersistedCtxStore<TKey, TState, TEvent, TParam, TCtx, TUserId> stateMachinePersistedCtxStore,
            IStateMachineMonadSettings settings)
        {
            this.stateMachinePersistedCtxStore = stateMachinePersistedCtxStore;
            this.expiresAfter = StopWatchThreadSafe.MillsToFrequency(settings.TimeOutMs);
            this.cleanAfter = StopWatchThreadSafe.MillsToFrequency(settings.CleanAfterMs);
        }
        
        public async Task<IStateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue>> To<TValue>(TKey key, TValue value)
        {
            this.MrClean();
            if (this.stateMachineCtx.TryGetValue(key, out var contextNode))
            {
                if (contextNode.Renew())
                {
                    return new StateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue>(value,
                        contextNode.Context);
                }
                else
                {
                    do
                    {
                        // DO Nothing here.  Something in the background is remove the node so we need to wait for this to finish.
                        Thread.Sleep(1); // Let other thread run.
                    } while (!this.stateMachineCtx.TryGetValue(key, out contextNode) || contextNode.IsActive);

                    if (this.stateMachineCtx.TryGetValue(key, out contextNode) && contextNode.IsActive)
                    {
                        return new StateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue>(value, contextNode.Context);
                    }
                    else
                    {
                        return await LoadDb(key, value);
                    }
                }
            }
            else
            {
                return await LoadDb(key, value);
            }
        }

        private void MrClean()
        {
            if (this.lastCleaned.Elapsed() > this.cleanAfter
                && Interlocked.CompareExchange(ref this.cleanerRunningState, CLEAN_RUNNING, CLEAN_IDLE) == CLEAN_IDLE)
            {
                this.lastCleaned.Reset();
                // Send to the thread pool so we don't block this request.
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    try
                    {
                        var contexts = this.stateMachineCtx.Values;
                        foreach (var context in contexts)
                        {
                            if (context.MarkForRemoval(this.expiresAfter))
                            {
                                this.stateMachineCtx.Remove(context.Context.StateMachineKey, out var ignore);
                                try
                                {
                                    context.Context.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex, ex.Message);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, ex.Message);
                    }
                    finally
                    {
                        Volatile.Write(ref this.cleanerRunningState, CLEAN_IDLE);
                    }
                });
            }
        }

        private async Task<StateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue>> LoadDb<TValue>(TKey key, TValue value)
        {
                var context = await this.stateMachinePersistedCtxStore.Load(key);
                if (context != null)
                {
                    this.stateMachineCtx.TryAdd(key, new ContextNode(context));
                    if (this.stateMachineCtx.TryGetValue(key, out var contextNode))
                    {
                        return new StateMachineMonad<TKey, TState, TEvent, TParam, TCtx, TUserId, TValue>(value, contextNode.Context);
                    }
                    else
                    {
                        throw new Exception("Unable to get the context.");
                    }
                }
                else
                {
                    throw new ArgumentException($"The context doesn't exists with the id of {key}", nameof(key));
                }
        }

        private struct ContextNode
        {
            private const int ACTIVE = 0;
            private const int REMOVING = 1;
            private const int REMOVED = 2;
            
            // TODO create the state machine to make sure we are returning an active context. 
            public readonly StopWatchThreadSafe LastUsed;
            public readonly TCtx Context;
            private int currentState;

            public ContextNode(TCtx context)
            {
                this.LastUsed = new StopWatchThreadSafe();
                this.Context = context;
                this.currentState = 0;
            }
            
            public bool IsActive
            {
                get
                {
                    return Volatile.Read(ref this.currentState) <= REMOVING;
                }
            }

            public bool MarkForRemoval(long timeIdle)
            {
                if (EligibleForRemoval(timeIdle) 
                    && Interlocked.CompareExchange(ref this.currentState, REMOVING, ACTIVE) == ACTIVE)
                {
                    Interlocked.MemoryBarrier(); // Can't have reordering here need a full fence instruction.
                    if (EligibleForRemoval(timeIdle)) // Need to check again so it's possible for another thread to trigger a run.
                    {
                        Volatile.Write(ref this.currentState, REMOVED);
                        return true;
                    }
                    else
                    {
                        Volatile.Write(ref this.currentState, ACTIVE);
                        return false;
                    }
                }

                return false;
            }

            public bool Renew()
            {
                if (this.IsActive)
                {
                    this.LastUsed.Reset();
                    Interlocked.MemoryBarrier(); // FULL Fence.
                    if (this.IsActive) // Need to check the state again just in case it got mark for deletion before we where able to reset it.
                    {
                        return true;
                    }
                }
                return false;
            }

            private bool EligibleForRemoval(long timeIdle)
            {
                return LastUsed.Elapsed() > timeIdle;
            }
        }
    }
}