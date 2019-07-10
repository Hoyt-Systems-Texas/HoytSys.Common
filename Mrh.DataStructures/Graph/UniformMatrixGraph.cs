using System;
using System.Collections.Generic;

namespace Mrh.DataStructures.Graph
{
    /// <summary>
    ///     A very basic matrix graph where its n x n matrix backend by an array for speed.  Do NOT use for very large graphs
    /// since it will use a ton of memory.
    /// </summary>
    public class UniformMatrixGraph
    {
        private readonly int length;
        private readonly bool[] graph;

        public UniformMatrixGraph(int length)
        {
            this.length = length;
            this.graph = new bool[length * length];
        }

        /// <summary>
        ///     Used to mark and edge.
        /// </summary>
        /// <param name="to">The to node.</param>
        /// <param name="from">The from node.</param>
        public void MarkEdge(int to, int from)
        {
            this.graph[CalculatePosition(to, from)] = true;
        }

        public void UnmarkEdge(int to, int from)
        {
            this.graph[CalculatePosition(to, from)] = false;
        }

        /// <summary>
        ///     Used to find the dependency graph and returns the order.
        /// </summary>
        public List<List<int>> FindDependencies()
        {
            // Need to find nodes without edges.
            var values = new List<List<int>>();
            var visited = new HashSet<int>();
            while (visited.Count < this.length)
            {
                var nodes = FindMatching(visited);
                if (nodes.Count == 0)
                {
                    throw new Exception("Cycle detected.");
                }
                values.Add(nodes);
            }
            return values;
        }

        private List<int> FindMatching(
            HashSet<int> visited)
        {
            var current = new List<int>();
            for (int i = 0; i < this.length; i++)
            {
                if (!visited.Contains(i))
                {
                    var found = false;
                    var start = i * length;
                    for (var j = 0; j < this.length; j++)
                    {
                        var pos = start + j;
                        if (!visited.Contains(j) && this.graph[pos])
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        current.Add(i);
                    }
                }
            }

            // Don't want to incur the IEnumerable performance penalty.
            for (var i = 0; i < current.Count; i++)
            {
                visited.Add(current[i]);
                
            }
            return current;
        }

        private int CalculatePosition(int to, int from)
        {
            return to * length + from;
        }
    }
}