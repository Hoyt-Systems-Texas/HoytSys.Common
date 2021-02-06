using System.Collections.Generic;

namespace HoytSys.DataStructures.Graph 
{
    /// <summary>
    ///     An immutable sparse graph design to fit into a small amount of memory.
    /// </summary>
    /// <typeparam name="TKey">The key for the edges.</typeparam>
    public class ImmutableSparseGraph<TKey>
    {

        private readonly TKey[] keys;
        private readonly ulong size;
        private readonly ulong[] edges;
        private readonly int keyCount;

        public ImmutableSparseGraph(
            TKey[] keys,
            int keyCount,
            ushort size,
            ulong[] edges)
        {
            this.keys = keys;
            this.keyCount = keyCount;
            this.size = size;
            this.edges = edges;
        }

        public static ImmutableSparseGraph<TKey> Create(
            IEnumerable<(TKey, TKey)> edges)
        {
            var dict = CreateLookup(edges);
            var number = dict.Count;

            return new ImmutableSparseGraph<TKey>(
                new TKey[0],
                0,
                0,
                new ulong[0]);
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