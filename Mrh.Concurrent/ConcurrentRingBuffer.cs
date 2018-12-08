using System.Threading;
using Mrh.Core;

namespace Mrh.Concurrent
{
    /// <summary>
    ///     A very simple ring buffer for storing objects in a queue.  Designed to be thread safe around multiple
    /// threads reading and write to the same buffer.  Takes advantage of .Net's struct for storing values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentRingBuffer<T> where T:class
    {

        private const int EMPTY = 0;

        private const int PENDING_SET = 1;

        private const int VALUE = 2;

        private const int PENDING_CLEAR = 3;

        private readonly ulong length;

        private readonly ulong mask;

        private readonly Node[] buffer;

        public ConcurrentRingBuffer(uint length)
        {
            this.length = (ulong) Pow2.NextPowerOf2((int) length);
            this.mask = this.length - 1;
            this.buffer = new Node[this.length];
        }

        public bool Set(ulong position, T value)
        {
            var pos = CalculatePosition(position);
            return this.buffer[pos].Set(value);
        }

        public bool Clear(ulong position)
        {
            var pos = CalculatePosition(position);
            return this.buffer[pos].Clear();
        }

        public bool TryGet(ulong position, out T value)
        {
            var pos = CalculatePosition(position);
            return this.buffer[pos].GetValue(out value);
        }

        private ulong CalculatePosition(ulong position)
        {
            return position & mask;
        }
        
        private struct Node
        {
            private int nodeValue;

            private T value;

            private long version;
            
            public bool Set(T value)
            {
                if (Interlocked.CompareExchange(ref this.nodeValue, PENDING_SET, EMPTY) == EMPTY)
                {
                    Volatile.Write(ref this.value, value);
                    Interlocked.Increment(ref this.version);
                    Volatile.Write(ref this.nodeValue, VALUE);
                    return true;
                }
                return false;
            }

            public bool Clear()
            {
                if (Interlocked.CompareExchange(ref this.nodeValue, PENDING_CLEAR, VALUE) == VALUE)
                {
                    Volatile.Write(ref this.value, default(T));
                    Volatile.Write(ref this.nodeValue, EMPTY);
                    return true;
                }
                return false;
            }

            public bool GetValue(out T value)
            {
                var version = Volatile.Read(ref this.version);
                value = Volatile.Read(ref this.value);
                if (Volatile.Read(ref this.nodeValue) == VALUE)
                {
                    return version == Volatile.Read(ref this.version);
                }
                return false;
            }
        }
    }
}