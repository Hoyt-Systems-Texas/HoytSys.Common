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

        private readonly Dictionary<TKey, ulong> pos;
        private readonly TKey[] keys;
        private readonly int keyCount;
        private readonly BitStore bitStore;
        private readonly ulong totalEdges;

        public ImmutableSparseGraph(
            Dictionary<TKey, ulong> values,
            TKey[] keys,
            int keyCount,
            BitStore bitStore,
            ulong totalEdges)
        {
            this.pos = values;
            this.keys = keys;
            this.keyCount = keyCount;
            this.bitStore = bitStore;
            this.totalEdges = totalEdges;
        }

        /// <summary>
        ///     Finds all of the pairs.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <param name="found">The function to call when the value is found.</param>
        public void Find(TKey value, Action<TKey> found)
        {
            if (this.pos.TryGetValue(value, out var idx))
            {
                var start = this.bitStore.BinarySearch(idx, 2);
                var count = (ulong) this.bitStore.Count;
                for (var i = start; i < count
                        && this.bitStore.Read(i) == idx; i+=2)
                {
                    var v = (int) this.bitStore.Read(i  + 1);
                    found(this.keys[v]);
                }
            }
        }
        
        /// <summary>
        ///     Used to create an immutable sparse graph.  Make sure the edges are
        /// already shorted by the first edge.
        /// </summary>
        /// <param name="edges"></param>
        /// <returns></returns>
        public static ImmutableSparseGraph<TKey> Create(
            IEnumerable<(TKey, TKey)> edges)
        {
            var dict = CreateLookup(edges);
            var count = dict.Count;
            var size = Pow2.MinimumBits(count);
            var keys = new TKey[count];
            foreach (var key in dict)
            {
                keys[key.Value] = key.Key;
            }
            var totalPairs = (ulong) edges.Count();
            var bitStore = new BitStore((int) (totalPairs * 2), size);
            var pos = 0ul;
            foreach (var (p, c) in edges)
            {
                bitStore.Write(pos++, dict[p]);
                bitStore.Write(pos++, dict[c]);
            }

            return new ImmutableSparseGraph<TKey>(
                dict,
                keys,
                count,
                bitStore,
                totalPairs);
        }

        /// <summary>
        ///     Creates a simple lookup dictionary for looking up the values.
        /// </summary>
        private static Dictionary<TKey, ulong> CreateLookup(IEnumerable<(TKey, TKey)> edges)
        {
            var dict = new Dictionary<TKey, ulong>(1000); 
            var id = 0ul;
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