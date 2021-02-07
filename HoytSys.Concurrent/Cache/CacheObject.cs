using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace A19.Concurrent.Cache
{
    public class CacheObject<T> where T:class
    {
        private T _value;
        private int _currentState = (int) CacheState.Pending;
        private readonly object _lock = new object();
        private List<TaskCompletionSource<T>> _pendingTasks = new List<TaskCompletionSource<T>>(5);
        private readonly Func<Task<T>> _loadFun;
        private readonly long _expiresAfterFq;
        private readonly StopWatchThreadSafe _stopWatchThreadSafe = new StopWatchThreadSafe();

        public CacheObject(
            Func<Task<T>> loadFun,
            TimeSpan timeSpan)
        {
            _loadFun = loadFun;
            _expiresAfterFq = StopWatchThreadSafe.MillsToFrequency((long) timeSpan.TotalMilliseconds);
        }

        public Task<T> Get()
        {
            if (CurrentState == CacheState.Active
                || CurrentState == CacheState.Reloading)
            {
                var result = Task.FromResult(Volatile.Read(ref _value));
                if (_stopWatchThreadSafe.Elapsed() > _expiresAfterFq)
                {
                    this.ChangeState(CacheEvent.Expire);
                }
                return result;
            }
            else
            {
                var task = new TaskCompletionSource<T>();
                lock (_lock)
                {
                    _pendingTasks.Add(task);
                }

                // Need to do this check since we can't do an atomic add.
                if (CurrentState == CacheState.Active)
                {
                    task.TrySetResult(_value);
                }
                this.ChangeState(CacheEvent.Load);
                return task.Task;
            }
        }

        private CacheState CurrentState
        {
            get { return (CacheState) Volatile.Read(ref _currentState); }
        }

        private void ChangeState(CacheEvent @event, T value = default)
        {
            switch (@event)
            {
                case CacheEvent.Load:
                    if (StateChange(CacheState.Pending, CacheState.Loading))
                    {
                        this.Load();
                    }
                    break;

                case CacheEvent.Expire:
                    if (StateChange(CacheState.Active, CacheState.Reloading))
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await this.Load();
                            }
                            catch (Exception)
                            {

                            }
                        });
                    }
                    break;

                case CacheEvent.Error:
                    // TODO figure out what to do on an error.
                    Volatile.Write(ref _currentState, 0);
                    break;
                
                case CacheEvent.Loaded:
                    if (StateChange(CacheState.Loading, CacheState.Loaded))
                    {
                        Volatile.Write(ref _value, value);
                        Volatile.Write(ref _currentState, (int) CacheState.Active);
                        _stopWatchThreadSafe.Reset();
                        lock (_lock)
                        {
                            foreach (var pendingTask in _pendingTasks)
                            {
                                pendingTask.SetResult(value);
                            }
                            _pendingTasks = new List<TaskCompletionSource<T>>(0);
                        }
                    } else if (Volatile.Read(ref _currentState) == (int) CacheState.Reloading)
                    {
                        // Don't need any ordering here.
                        Volatile.Write(ref _value, value);
                        Volatile.Write(ref _currentState, (int) CacheState.Active);
                        _stopWatchThreadSafe.Reset();
                    }
                    break;
            }
        }
        
        private bool StateChange(CacheState currentState, CacheState newState)
        {
            var cI = (int) currentState;
            var nI = (int) newState;
            return Interlocked.CompareExchange(
                       ref _currentState,
                       nI,
                       cI) == cI;
        }

        private async Task Load()
        {
            var r = await _loadFun();
            this.ChangeState(CacheEvent.Loaded, r);
        }

        public enum CacheState
        {
            Pending = 0,
            Loading = 1,
            Active = 2,
            Error = 3,
            Reloading = 4,
            Loaded = 5
        }

        public enum CacheEvent
        {
            Load = 0,
            Expire = 1,
            Error = 2,
            Loaded = 3
        }
    }
}