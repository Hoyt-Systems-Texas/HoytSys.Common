using System;
using System.Threading;
using System.Threading.Tasks;

namespace A19.Concurrent.Cache
{
    /// <summary>
    ///     A cached that supports a version number to signal when it should be reloaded.
    /// </summary>
    public class CachedVersion<T> where T:class
    {
        private const int Pending = 0;
        private const int Loading = 1;
        private const int Loaded = 2;
        private const int Reloading = 3;
        
        private const int Error = 10;

        private int state = Pending;
        private long version = 0;
        private T value = default(T);
        private readonly Func<Task<T>> loadFunction;

        public CachedVersion(
            Func<Task<T>> loadFunction)
        {
            this.loadFunction = loadFunction;
        }

        public void Expire()
        {
            Interlocked.Increment(ref this.version);
        }

        public void Reload()
        {
            
        }
    }
}