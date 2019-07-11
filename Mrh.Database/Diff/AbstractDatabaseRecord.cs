using System.Threading;

namespace Mrh.Database.Diff
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

        public const int SAME = 10;
        public const int NEW = 11;
        public const int UPDATE = 12;
        public const int DELETE = 13;

        private int currentState = PENDING;
        private int updateType = SAME;

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

        public bool NewRecord()
        {
            return Interlocked.CompareExchange(
                       ref this.currentState,
                       NEW,
                       SAME) == SAME;
        }

        public bool UpdateRecord()
        {
            return Interlocked.CompareExchange(
                       ref this.updateType,
                       UPDATE,
                       SAME) == SAME;
        }

        public bool DeleteRecord()
        {
            return Interlocked.CompareExchange(
                       ref this.updateType,
                       DELETE,
                       SAME) == SAME;
        }

        public bool IsNew()
        {
            return this.UpdateType == NEW;
        }

        public bool IsUpdate()
        {
            return this.UpdateType == UPDATE;
        }

        public int UpdateType => updateType;

        /// <summary>
        ///     Called when the record is saved.
        /// </summary>
        public void Saved()
        {
            Interlocked.Exchange(ref this.currentState, SAVED);
        }
        
    }
}