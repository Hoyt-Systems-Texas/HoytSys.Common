using System.Threading;
using HoytSys.Core;

namespace A19.Concurrent
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

        public readonly long Length;

        public readonly long Mask;

        private readonly Node[] buffer;

        public ConcurrentRingBuffer(uint length)
        {
            if (!Pow2.IsPowerOfTwo((int) length))
            {
                this.Length = Pow2.NextPowerOf2((int) length);
            }
            this.Length = length;
            this.Mask = this.Length - 1;
            this.buffer = new Node[this.Length];
        }

        public bool Set(long position, T value)
        {
            var pos = CalculatePosition(position);
            return this.buffer[pos].Set(value);
        }

        public bool Clear(long position)
        {
            var pos = CalculatePosition(position);
            return this.buffer[pos].Clear();
        }

        public bool TryGet(long position, out T value)
        {
            var pos = CalculatePosition(position);
            return this.buffer[pos].GetValue(out value);
        }

        public bool HasValue(long position)
        {
            var pos = CalculatePosition(position);
            return this.buffer[pos].HasValue();
        }

        public bool IsEmpty(long position)
        {
            var pos = CalculatePosition(position);
            return this.buffer[pos].IsEmpty();
        }

        private long CalculatePosition(long position)
        {
            return position & Mask;
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

            public bool HasValue()
            {
                return Volatile.Read(ref this.nodeValue) == VALUE;
            }

            public bool IsEmpty()
            {
                return Volatile.Read(ref this.nodeValue) == EMPTY;
            }
        }
    }
}