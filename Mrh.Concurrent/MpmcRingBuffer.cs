using System.Diagnostics;

namespace Mrh.Concurrent
{
    public class MpmcRingBuffer<T> where T:class
    {
        private readonly ConcurrentRingBuffer<T> buffer;

        /// <summary>
        ///  Here to prevent false sharing.
        /// </summary>
        private readonly PaddedLong ____additionalPadding;

        private PaddedLong producerIndex;

        private PaddedLong consumuerIndex;
        
        public MpmcRingBuffer(uint size)
        {
            this.buffer = new ConcurrentRingBuffer<T>(size);
        }

        public bool Offer(T value)
        {
            var capacity = this.buffer.Length;
            long pIndex;
            long consumerIndex;
            do
            {
                pIndex = this.producerIndex.VolatileRead();
                consumerIndex = this.consumuerIndex.VolatileRead();
                if (pIndex - capacity >= consumerIndex)
                {
                    return false;
                }
            } while (this.buffer.HasValue(pIndex + 1) // Need to go around again if a value is still at that position.
                || !this.producerIndex.CompareExchange(pIndex + 1, pIndex));

            var set = this.buffer.Set(pIndex + 1, value);
            Debug.Assert(set, "Did not find an open position correctly");
            return true;
        }
        
        public bool TryPoll(out T value) 
        {
            long pIndex;
            long consumerIndex;
            do
            {
                pIndex = this.producerIndex.VolatileRead();
                consumerIndex = this.consumuerIndex.VolatileRead();
                if (consumerIndex >= pIndex)
                {
                    value = default(T);
                    return false;
                }
            } while (!this.consumuerIndex.CompareExchange(consumerIndex + 1, consumerIndex));

            var pos = consumerIndex + 1;
            var get = this.buffer.TryGet(pos, out value);
            this.buffer.Clear(pos);
            Debug.Assert(get, "Failed to set the value.");
            return true;
        }

        /// <summary>
        ///     Tries to get the current value in the queue.
        /// </summary>
        /// <param name="value">The value at that position.</param>
        /// <returns>true if a value is at that position.</returns>
        public bool TryPeek(out T value)
        {
            return this.buffer.TryGet(this.consumuerIndex.VolatileRead(), out value);
        }

    }
}