using System.Collections.Generic;

namespace HoytSys.DataStructures
{
    public interface IMultiMap<TKey, TValue, TContainerType> : IEnumerable<KeyValuePair<TKey, TValue>> where TContainerType: IEnumerable<TValue>
    {
        
        /// <summary>
        ///     Used to get the results.
        /// </summary>
        /// <param name="index"></param>
        TContainerType this[TKey index] { get; }

        void Put(TKey key, TValue value);

        void PutAll(TKey key, IEnumerable<TValue> values);

        IEnumerable<TKey> Keys { get; }
        
        IEnumerable<TValue> Values { get; }
        
        /// <summary>
        ///     The number of keys in the collection
        /// </summary>
        int Count { get; }

        /// <summary>
        ///      The type of key returned.
        /// </summary>
        /// <param name="key">The key type.</param>
        /// <returns>true if the collection contains the key.</returns>
        bool ContainsKey(TKey key);

    }
}