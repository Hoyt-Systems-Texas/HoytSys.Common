using System.Threading;

namespace Mrh.Database
{
    /// <summary>
    ///     All diff record must inherit this class inorder to do the updates.
    /// </summary>
    /// <typeparam name="TKey">The type for the key.</typeparam>
    public abstract class AbstractDatabaseRecord<TKey>
    {
        public abstract TKey Id { get; }

        private const int PENDING = 0;
        private const int SAVING = 1;
        private const int SAVED = 2;

        private int currentState = PENDING;

        /// <summary>
        ///     They think it should save.
        /// </summary>
        /// <returns>true when the value if it's suppose to be saved.</returns>
        public bool Save()
        {
            return Interlocked.CompareExchange(
                       ref this.currentState,
                       SAVING,
                       PENDING) == PENDING;
        }

        /// <summary>
        ///     Called when the record is saved.
        /// </summary>
        public void Saved()
        {
            Interlocked.Exchange(ref this.currentState, SAVED);
        }
        
    }
}