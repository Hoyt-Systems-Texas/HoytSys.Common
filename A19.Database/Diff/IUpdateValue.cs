namespace A19.Database.Diff
{
    
    /// <summary>
    ///     Represents the base type for updating records.
    /// </summary>
    /// <typeparam name="TNew">The new value for the comparision.</typeparam>
    /// <typeparam name="TDb">The type of the database record.</typeparam>
    /// <typeparam name="TKey">The type of the key value for the database.</typeparam>
    public interface IUpdateValue<TNew, TDb, TKey> where TDb:AbstractDatabaseRecord<TKey>
    {
        
        /// <summary>
        ///  Returns true if the value has changed.
        /// </summary>
        /// <returns>true if the value has changed.</returns>
        bool Update(TNew newValue, TDb value);
        
    }
}