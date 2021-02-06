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
        private readonly int keyCount;
        private readonly BitStore bitStore;

        public ImmutableSparseGraph(
            TKey[] keys,
            int keyCount,
            BitStore bitStore)
        {
            this.keys = keys;
            this.keyCount = keyCount;
            this.bitStore = bitStore;
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
                keys,
                count,
                bitStore);
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