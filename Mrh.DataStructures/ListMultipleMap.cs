using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mrh.DataStructures
{
    /// <summary>
    ///     A basic list map implementation.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class ListMultipleMap<TKey, TValue> : IMultiMap<TKey, TValue, IReadOnlyList<TValue>>
    {

        private readonly Dictionary<TKey, List<TValue>> contents;

        private readonly int defaultListSize;
        
        private readonly List<TValue> defaultEmpty = new List<TValue>(0);

        public ListMultipleMap(int defaultKeyLength, int defaultListSize)
        {
            this.contents = new Dictionary<TKey, List<TValue>>(defaultKeyLength);
            this.defaultListSize = defaultListSize;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var listPair in this.contents)
            {
                foreach (var value in listPair.Value)
                {
                    yield return new KeyValuePair<TKey, TValue>(listPair.Key, value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IReadOnlyList<TValue> this[TKey index]
        {
            get
            {
                if (this.ContainsKey(index))
                {
                    return this.contents[index];
                }

                return this.defaultEmpty;
            }
        }

        public void Put(TKey index, TValue value)
        {
            if (!this.ContainsKey(index))
            {
                this.contents[index] = new List<TValue>(this.defaultListSize);
            }
            this.contents[index].Add(value);
        }

        public void PutAll(TKey key, IEnumerable<TValue> values)
        {
            if (!this.ContainsKey(key))
            {
                this.contents[key] = new List<TValue>(this.defaultListSize);
            }
            this.contents[key].AddRange(values);
        }

        public IEnumerable<TKey> Keys => contents.Keys;

        public IEnumerable<TValue> Values {
            get
            {
                foreach (var content in this.contents)
                {
                    foreach (var value in content.Value)
                    {
                        yield return value;
                    }
                }
            }
        }

        public int Count => this.contents.Count;
        
        public bool ContainsKey(TKey key)
        {
            return this.contents.ContainsKey(key);
        }
    }
}