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
                this.Find(idx, (val) =>
                {
                    found(this.keys[(int)val]);
                });
            }
        }

        private void Find(ulong idx, Action<ulong> found)
        {
            var start = this.bitStore.BinarySearch(idx, 2);
            var count = (ulong) this.bitStore.Count;
            for (var i = start; i < count
                    && this.bitStore.Read(i) == idx; i+=2)
            {
                var v = this.bitStore.Read(i  + 1);
                found(v);
            }
        }

        /// <summary>
        ///     Used to find a path from the starting point to the end point.  This finds the
        /// most optimal path ad the expense of time and memory.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        public List<TKey> Bfs(TKey startKey, TKey endKey)
        {
            if (this.pos.TryGetValue(startKey, out var start)
                && this.pos.TryGetValue(endKey, out var end))
            {
                if (start == end)
                {
                    return new List<TKey>
                    {
                        startKey
                    };
                }
                else
                {
                    // Keep track of the previous nodes ot prevent a cycle.
                    var previousNodes = new HashSet<ulong>();
                    // The queue to use for the BFS
                    var queue = new Queue<(ulong, BitStore)>();
                    // The starting vector to use.
                    var startVec = this.bitStore.CreateNew(1);
                    startVec.Write(0ul, start);
                    queue.Enqueue((0ul, startVec));
                    previousNodes.Add(start);
                    while (true)
                    {
                        if (queue.TryDequeue(out var value))
                        {
                            // Get the current path for the node.
                            var (last, path) = value;
                            // Read the current value at that position.
                            var currentNode = path.Read(last);
                            // If we find a path, this is where it will be.
                            List<TKey> foundPath = null;
                            this.Find(currentNode, newNode =>
                            {
                                if (foundPath != null)
                                {
                                   // Do Nothing since path is found
                                }
                                else if (newNode == end)
                                {
                                    // Found the path and now need to get the keys represented by the number.
                                    foundPath = new List<TKey>();
                                    for (var i = 0ul; i < (ulong) path.Count; i++)
                                    {
                                        var val = path.Read(i);
                                        foundPath.Add(this.keys[val]);
                                    }
                                    foundPath.Add(this.keys[newNode]);
                                }
                                else
                                {
                                    // Check to see if we already visited a node.
                                    // Since we support graphs we need to support cycles.
                                    if (previousNodes.Add(newNode))
                                    {
                                        // Create a copy of the current path.
                                        var newPath = path.Clone(path.Count + 1);
                                        var pos = last + 1;
                                        // Write the new path.
                                        newPath.Write(pos, newNode);
                                        queue.Enqueue((pos, newPath));
                                    }
                                }
                            });
                            if (foundPath != null)
                            {
                                return foundPath;
                            }
                        }
                        else
                        {
                            return new List<TKey>(0);
                        }
                    }
                }
            }
            else
            {
                return new List<TKey>(0);
            }
        }

        /// <summary>
        ///     Used to get the minimum spanning tree from the starting value.
        /// </summary>
        /// <param name="start">The starting value to get the minimum spanning tree.</param>
        /// <returns>The list of edges for the minimum spanning tree.</returns>
        public List<(TKey, TKey)> MinimumSpanningTree(TKey startKey)
        {
            // Pretty much the same as breath first search except we stop when
            // their nothing left in the queue.
            if (this.pos.TryGetValue(startKey, out var start))
            {
                // Keep track of the previous nodes ot prevent a cycle.
                var previousNodes = new HashSet<ulong>();
                // The queue to use for the BFS
                var queue = new Queue<ulong>();
                // Enqueue the first value.
                queue.Enqueue(start);
                previousNodes.Add(start);
                List<(TKey, TKey)> edges = new List<(TKey, TKey)>(16);
                while (true)
                {
                    if (queue.TryDequeue(out var currentNode))
                    {
                        this.Find(currentNode, newNode =>
                        {
                            // Check to verify we don't have cycles and we don't want to
                            // add them since we want a tree.
                            if (previousNodes.Add(newNode))
                            {
                                edges.Add((this.keys[currentNode], this.keys[newNode]));
                                queue.Enqueue(newNode);
                            }
                        });
                    }
                    else
                    {
                        return edges;
                    }
                }
            }
            else
            {
                // Doesn't contain a tree so return empty list.
                return new List<(TKey, TKey)>(0);
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
            // Create a dictionary lookup for the key to value.
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
            // Make sure we add the first value sin order.
            foreach (var (v1, _) in edges)
            {
                if (!dict.ContainsKey(v1)) 
                {
                    dict[v1] = id++;
                }
            }

            // Add any remaining values.
            foreach (var (_, v2) in edges)
            {
                if (!dict.ContainsKey(v2))
                {
                    dict[v2] = id++;
                }
            }
            return dict;
        }
    }
}