using System.Threading;

namespace Mrh.Concurrent
{
    /// <summary>
    ///     An ultra fast semaphore that atomic operations to acquire a lock on the semaphore.
    /// </summary>
    public class FastSemaphore
    {

        /// <summary>
        ///     The current number of things we are trying to protect.
        /// </summary>
        private long currentNumber;

        /// <summary>
        ///     The number of times it's been release.:w
        /// </summary>
        private long releaseNumber;
        
        /// <summary>
        ///     The maximum number we are aloud to have.
        /// </summary>
        private readonly long max;

        /// <summary>
        /// </summary>
        /// <param name="max">The maximum of acquire aloud.</param>
        public FastSemaphore(int max)
        {
            this.max = max;
        }

        public bool AcquireFailFast()
        {
            var current = Volatile.Read(ref this.currentNumber);
            var release = Volatile.Read(ref this.releaseNumber);
            var total = current - release;
            if (total < max)
            {
                return Interlocked.CompareExchange(ref this.currentNumber, current + 1, current) == current;
            }

            return false;
        }

        public void Release()
        {
            Interlocked.Increment(ref this.releaseNumber);
        }
        
    }
}