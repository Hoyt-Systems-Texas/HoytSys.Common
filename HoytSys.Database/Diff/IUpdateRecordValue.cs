using System.Collections.Generic;

namespace A19.Database.Diff
{
    public interface IUpdateRecordValue<TNew, TDb, TKey, TUserId> where TDb: AbstractDatabaseRecord<TKey, TNew>
    {
    
        bool Immutable { get; }
        
        /// <summary>
        ///     Called on a complex object to update the values.
        /// </summary>
        /// <param name="newValue">The new value to update.</param>
        /// <param name="value">The value we are updating.</param>
        /// <param name="updateValues">The multi-map containing the values to update.</param>
        /// <returns>true if the value has changed.</returns>
        bool Update(TNew newValue, TDb value, Dictionary<int, IUpdateRecords<TUserId>> updateValues);
    }
}