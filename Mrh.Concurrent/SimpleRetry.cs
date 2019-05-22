using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mrh.DataStructures;

namespace Mrh.Concurrent
{
    public class SimpleRetry : IRetryService
    {
        private const int STOPPED = 0;
        private const int STARTING = 1;
        private const int RUNNING = 2;
        private const int STOPPING = 3;

        private readonly BinaryHeap<RetryNode> queue;
        private readonly MpmcRingBuffer<Action> runQueue;
        private readonly Action<Exception> exceptionHandler;
        private int currentState = STOPPED;
        private Thread thread;

        public SimpleRetry(
            int defaultSize,
            Action<Exception> exceptionHandler)
        {
            this.queue = new BinaryHeap<RetryNode>(defaultSize, new RetryNodeComparer());
            this.runQueue = new MpmcRingBuffer<Action>(0x100);
            this.exceptionHandler = exceptionHandler;
        }

        public void Start()
        {
            if (Interlocked.CompareExchange(
                    ref this.currentState,
                    STARTING, STOPPED)
                == STOPPED)
            {
                this.thread = new Thread(Main);
                this.thread.Start();
            }
        }

        public void Stop()
        {
            if (Interlocked.CompareExchange(
                    ref this.currentState,
                    STOPPING,
                    RUNNING) == RUNNING)
            {
                
            }
        }

        public void Retry(TimeSpan retry, Action act)
        {
            this.queue.Put(new RetryNode(
                act, 
                DateTime.Now.Add(retry)));
        }

        private void Main()
        {
            Interlocked.Exchange(ref this.currentState, RUNNING);
            var spin = new SpinWait();
            while (true)
            {
                try
                {
                    if (Volatile.Read(ref this.currentState) == STOPPING)
                    {
                        break;
                    }

                    Action act;
                    var current = DateTime.Now;
                    RetryNode node;
                    bool ran = false;
                    if (this.runQueue.TryPoll(out act))
                    {
                        ran = true;
                        act();
                    }

                    if (this.queue.Peek(out node)
                        && node.RunDate < current)
                    {
                        this.queue.Take(out node);
                        ThreadPool.QueueUserWorkItem((obj) =>
                        {
                            node.Act();
                        });
                        ran = true;
                    }

                    if (!ran)
                    {
                        spin.SpinOnce();
                    }
                }
                catch (Exception ex)
                {
                    // TODO add an exception strategy.
                    this.exceptionHandler(ex);
                }
            }

            Interlocked.Exchange(ref this.currentState, STOPPED);
        }

        private struct RetryNode
        {
            public readonly Action Act;

            public readonly DateTime RunDate;

            public RetryNode(
                Action act,
                DateTime date)
            {
                this.Act = act;
                this.RunDate = date;
            }
        }

        private class RetryNodeComparer : IComparer<RetryNode>
        {
            public int Compare(RetryNode x, RetryNode y)
            {
                return x.RunDate.CompareTo(y.RunDate);
            }
        }
    }
}