using System;
using System.Collections.Generic;
using System.Linq;
using HoytSys.Core;

namespace HoytSys.DataStructures.Graph 
{
    /// <summary>
    ///     An immutable sparse graph design to fit into a small amount of memory.
    /// </summary>
    /// <typeparam name="TKey">The key for the edges.</typeparam>
    public class ImmutableSparseGraph<TKey>
    {

        private readonly TKey[] keys;
        private readonly ulong mask;
        private readonly ulong bits;
        private readonly ulong[] edges;
        private readonly int keyCount;

        public ImmutableSparseGraph(
            TKey[] keys,
            int keyCount,
            ulong mask,
            ulong bits,
            ulong[] edges)
        {
            this.keys = keys;
            this.keyCount = keyCount;
            this.mask = mask;
            this.bits = bits;
            this.edges = edges;
        }

        public static ImmutableSparseGraph<TKey> Create(
            IEnumerable<(TKey, TKey)> edges)
        {
            var dict = CreateLookup(edges);
            var count = dict.Count;
            var size = Pow2.MinimumBits(count);
            var mask = (ulong) 1 << size - 1;
            var keys = new TKey[count];
            foreach (var key in dict)
            {
                keys[key.Value] = key.Key;
            }
            var totalPairs = (ulong) edges.Count();
            var totalSizeBits = totalPairs * 2 * (ulong) size;
            var totalSize = (int) Math.Ceiling(totalSizeBits / 64.0);
            var edgesArray = new ulong[totalSize];

            return new ImmutableSparseGraph<TKey>(
                keys,
                count,
                mask,
                (ulong) size,
                edgesArray);
        }

        /// <summary>
        ///     Creates a simple lookup dictionary for looking up the values.
        /// </summary>
        private static Dictionary<TKey, int> CreateLookup(IEnumerable<(TKey, TKey)> edges)
        {
            var dict = new Dictionary<TKey, int>(1000); 
            var id = 0;
            foreach (var (v1, v2) in edges)
            {
                if (!dict.ContainsKey(v1)) 
                {
                    dict[v1] = id++;
                }
                if (!dict.ContainsKey(v2))
                {
                    dict[v2] = id++;
                }
            }
            return dict;
        }
    }
}