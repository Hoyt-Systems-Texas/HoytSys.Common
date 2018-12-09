using System;
using System.Diagnostics;

namespace Mrh.Concurrent
{
    public class MpmcRingBuffer<T> where T:class
    {
        private readonly ConcurrentRingBuffer<T> buffer;

        /// <summary>
        ///  Here to prevent false sharing.
        /// </summary>
#pragma warning disable 169
        private readonly PaddedLong ____additionalPadding;
#pragma warning restore 169

        private PaddedLong producerIndex = new PaddedLong();

        private PaddedLong consumerIndex = new PaddedLong();
        
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
                consumerIndex = this.consumerIndex.VolatileRead();
                if (pIndex - capacity >= consumerIndex)
                {
                    return false;
                }
            } while (!this.buffer.IsEmpty(pIndex) // Need to go around again if a value is still at that position.
                || !this.producerIndex.CompareExchange(pIndex + 1, pIndex));

            var set = this.buffer.Set(pIndex, value);
            Debug.Assert(set, $"Did not find an open position correctly {pIndex}");
            return true;
        }
        
        public bool TryPoll(out T value) 
        {
            long pIndex;
            long consumerIndex;
            do
            {
                pIndex = this.producerIndex.VolatileRead();
                consumerIndex = this.consumerIndex.VolatileRead();
                if (consumerIndex >= pIndex)
                {
                    value = default(T);
                    return false;
                }
            } while (!this.buffer.HasValue(consumerIndex) || !this.consumerIndex.CompareExchange(consumerIndex + 1, consumerIndex));

            var get = this.buffer.TryGet(consumerIndex, out value);
            this.buffer.Clear(consumerIndex);
            Debug.Assert(get, $"Failed to get the value. {consumerIndex}");
            return true;
        }

        /// <summary>
        ///     Tries to get the current value in the queue.
        /// </summary>
        /// <param name="value">The value at that position.</param>
        /// <returns>true if a value is at that position.</returns>
        public bool TryPeek(out T value)
        {
            return this.buffer.TryGet(this.consumerIndex.VolatileRead(), out value);
        }

        public int Drain(Action<T> act, int limit)
        {
            long consumerIndex;
            long pIndex;
            for (var i = 0; i < limit; i++)
            {
                do
                {
                    pIndex = this.producerIndex.VolatileRead();
                    consumerIndex = this.consumerIndex.VolatileRead();
                    if (consumerIndex >= pIndex)
                    {
                        return i;
                    }
                } while (!buffer.HasValue(consumerIndex)
                         || !this.consumerIndex.CompareExchange(
                             consumerIndex + 1,
                             consumerIndex));

                var success = this.buffer.TryGet(consumerIndex, out T value);
                Debug.Assert(success, "Did not successfully get the value in a drain");
                act(value);
                success = this.buffer.Clear(consumerIndex);
                Debug.Assert(success, "Unable to clear the value on a drain");
            }
            return limit;
        }
        
    }
}