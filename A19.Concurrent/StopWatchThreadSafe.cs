using System.Diagnostics;
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
            var frequency = QueryPerformanceFrequency();
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
        
    [DllImport("System.Native", EntryPoint = "SystemNative_GetTimestamp")]
        internal static extern long QueryPerformanceCounter();
        
    [DllImport("System.Native", EntryPoint = "SystemNative_GetTimestampResolution")]
        internal static extern long QueryPerformanceFrequency();
        
        public static long GetTimestamp()
        {
          return QueryPerformanceCounter();
        }
    }
}