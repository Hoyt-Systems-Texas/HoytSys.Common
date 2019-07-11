using System.Runtime.InteropServices;
using System.Threading;

namespace A19.Concurrent
{
    /// <summary>
    ///     A thread safe stop watch safe to call from multiple threads.
    /// </summary>
    public class StopWatchThreadSafe
    {

        private static readonly double tickFrequency;

        private const long TICKS_IN_MILLS = 10000;

        private long start;

        static StopWatchThreadSafe()
        {
            QueryPerformanceFrequency(out long frequency);
            tickFrequency = 10000000.0;
            tickFrequency /= (double) frequency;
        }

        public StopWatchThreadSafe()
        {
            this.start = GetTimestamp();
        }

        /// <summary>
        ///     Used to reset the timer.
        /// </summary>
        public void Reset()
        {
            Volatile.Write(ref this.start, GetTimestamp());
        }

        /// <summary>
        ///     The amount of elapsed time that has gone by.
        /// </summary>
        /// <returns></returns>
        public long Elapsed()
        {
            return GetTimestamp() - Volatile.Read(ref this.start);
        }

        public static long MillsToFrequency(long milliseconds)
        {
            var value = milliseconds * TICKS_IN_MILLS;
            return (long) (value / tickFrequency);
        }
        
        [DllImport("kernel32.dll")]
        internal static extern bool QueryPerformanceCounter(out long value);
        
        [DllImport("kernel32.dll")]
        internal static extern bool QueryPerformanceFrequency(out long value);
        
        public static long GetTimestamp()
        {
          QueryPerformanceCounter(out long value);
          return value;
        }
    }
}