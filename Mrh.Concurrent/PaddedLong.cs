using System.Threading;

namespace Mrh.Concurrent
{
    public struct PaddedLong
    {
        private long Value;
        private long v1;
        private long v2;
        private long v3;
        private long v4;
        private long v5;
        private long v6;
        private long v7;
        private long v8;
        private long v9;
        private long v10;
        private long v11;
        private long v12;
        private long v13;
        private long v14;
        private long v15;

        public long VolatileRead()
        {
            return Volatile.Read(ref this.Value);
        }

        public long Increment()
        {
            return Interlocked.Increment(ref this.Value);
        }

        public bool CompareExchange(long newValue, long compareValue)
        {
            return Interlocked.CompareExchange(ref this.Value, newValue, compareValue) == compareValue;
        }

        public void VolatileWrite(long value)
        {
            Volatile.Write(ref this.Value, value);
        }
    }
}