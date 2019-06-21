using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Mrh.Concurrent
{
    /// <summary>
    ///     A current array list to insert/remove a new value.  Insert best case is O(1) unless it needs to grow then it's O(n).  Removes
    /// are O(n) worst case.
    /// </summary>
    public class ConcurrentArrayBag<T> : IEnumerable<T> where T : class
    {
        /*
         * Need to decided if we want to do something more complicated where the other threads will wait and just use locks or use a combination since only writes can have a problem not reads.
         */

        /// <summary>
        ///     THe current state of the array.
        /// </summary>
        private int state = 0;

        private const int IDLE = 0;
        private const int EXPANDING = 1;
        private const int SHRINKING = 2;

        private const int DOUBLE_UP_TO = 1000;
        private const double GROWTH_FACTOR = 2;

        private const int AVAILABLE = 0;
        private const int SETTING = 1;
        private const int CLEARING = 2;
        private const int VALID = 10;
        private const int MARKED_FOR_REMOVAL = 11;

        private int count = 0;
        private int currentIndex = -1;

        private ArrayNode[] nodes;

        private readonly int defaultSize;

        /// <summary>
        ///     Used to create a new array bag.
        /// </summary>
        /// <param name="size">The size of the bag to create.</param>
        public ConcurrentArrayBag(int size)
        {
            this.defaultSize = size;
            this.nodes = new ArrayNode[size];
            for (int i = 0; i < size; i++)
            {
                this.nodes[i] = new ArrayNode();
            }
        }


        /// <summary>
        ///     Used to add a value to the bag.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Add(T value)
        {
            var nextPosition = Interlocked.Increment(ref this.currentIndex);
            while (true)
            {
                var currentNodes = Volatile.Read(ref this.nodes);
                if (nextPosition < currentNodes.Length)
                {
                    if (currentNodes[nextPosition].TrySet(value))
                    {
                        Interlocked.Increment(ref this.count);
                        return;
                    }
                }

                // Need to make sure it rereads the value each time.
                if (!this.Grow())
                {
                    Thread.Yield();
                }
            }
        }

        /// <summary>
        ///     Removes a value form the bag.
        /// </summary>
        /// <param name="compareFunc">The function to use to see if we should delete the node.</param>
        /// <returns>The number of items removed.</returns>
        public int Remove(Func<T, bool> compareFunc)
        {
            var currentNodes = Volatile.Read(ref this.nodes);
            int count = 0;
            foreach (var node in currentNodes)
            {
                T value;
                if (node.TryGet(out value)
                    && compareFunc(value))
                {
                    if (node.MarkForRemoval())
                    {
                        count++;
                        Interlocked.Decrement(ref this.count);
                    }
                }
            }

            return count;
        }

        /// <summary>
        ///     Called when there is no more remove left.
        /// </summary>
        private bool Grow()
        {
            // Only 1 thread is aloud to grow/schrink the data structure.
            if (Interlocked.CompareExchange(
                    ref this.state,
                    EXPANDING,
                    IDLE) == IDLE)
            {
                var currentNodes = Volatile.Read(ref this.nodes);
                var arrayLength = currentNodes.Length;
                var currentSize = Math.Max(
                    Math.Min(
                        arrayLength,
                        Volatile.Read(ref this.currentIndex)),
                    this.defaultSize);
                currentSize = Math.Max(arrayLength, currentSize);
                var newSize = currentSize * DOUBLE_UP_TO;
                Debug.Assert(newSize >= currentNodes.Length);
                if (newSize > DOUBLE_UP_TO)
                {
                    newSize = (int) Math.Floor(currentSize * GROWTH_FACTOR);
                }

                var newArray = new ArrayNode[newSize];
                for (var i = 0; i < newSize; i++)
                {
                    if (i < arrayLength
                        && !currentNodes[i].IsRemoved())
                    {
                        newArray[i] = currentNodes[i];
                    }
                    else
                    {
                        newArray[i] = new ArrayNode();
                    }
                }

                // We need the write barriers for this to work.
                Volatile.Write(ref this.nodes, newArray);
                // Set the state back to idle.
                Volatile.Write(ref this.state, IDLE);
                return true;
            }

            return false;
        }

        private class ArrayNode
        {
            /// <summary>
            ///     The current state of the node.
            /// </summary>
            public int State;

            /// <summary>
            ///     The value at the node.
            /// </summary>
            public T Value;

            /// <summary>
            ///     The version number.
            /// </summary>
            public int Version;

            /// <summary>
            ///     Get the value at the node.
            /// </summary>
            /// <param name="value">The value at that position.</param>
            /// <returns>false if the value at that position is invalid.</returns>
            public bool TryGet(out T value)
            {
                // Order extremely import on how this is read.
                var versionId = Volatile.Read(ref this.Version);
                if (Volatile.Read(ref this.State) == VALID)
                {
                    value = Volatile.Read(ref this.Value);
                    if (Volatile.Read(ref this.Version) == versionId
                        && Volatile.Read(ref this.State) == VALID)
                    {
                        return true;
                    }
                }

                value = default;
                return false;
            }

            /// <summary>
            ///     Tries to get a value at the position.
            /// </summary>
            /// <param name="value">The value at the position.</param>
            /// <returns>true if the value is set.</returns>
            public bool TrySet(T value)
            {
                // Do not change the order of these operations.
                if (Interlocked.CompareExchange(
                        ref this.State,
                        SETTING,
                        AVAILABLE) == AVAILABLE)
                {
                    Interlocked.Increment(ref this.Version);
                    Volatile.Write(ref this.Value, value);
                    Volatile.Write(ref this.State, VALID);
                    return true;
                }

                return false;
            }

            /// <summary>
            ///     Mark the node for removal.
            /// </summary>
            /// <returns>Marks the node for removal when doing a cleaning operations.</returns>
            public bool MarkForRemoval()
            {
                return Interlocked.CompareExchange(
                           ref this.State,
                           MARKED_FOR_REMOVAL,
                           VALID) == VALID;
            }

            /// <summary>
            ///     The node has been marked for removal.
            /// </summary>
            /// <returns>Marks the node for removal.</returns>
            public bool IsRemoved()
            {
                return Volatile.Read(ref this.State) == MARKED_FOR_REMOVAL;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var currentNodes = Volatile.Read(ref this.nodes);
            foreach (var node in currentNodes)
            {
                T value;
                if (node.TryGet(out value))
                {
                    yield return value;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}