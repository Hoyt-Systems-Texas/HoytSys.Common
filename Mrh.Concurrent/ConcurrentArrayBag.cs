using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Mrh.Concurrent
{
    /// <summary>
    ///     A current array list to insert/remove a new value.  Designed for very small list and has O(n) performance.  Would require a different implementation for speed.
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

        private ArrayNode[] nodes;

        /// <summary>
        ///     Used to create a new array bag.
        /// </summary>
        /// <param name="size">The size of the bag to create.</param>
        public ConcurrentArrayBag(int size)
        {
            this.nodes = new ArrayNode[size];
            for (int i = 0; i < size; i++)
            {
                this.nodes[i] = new ArrayNode();
            }
        }

        
        /// <summary>
        ///     Used to add a value to the bag.
        /// </summary>
        /// <param name="value"></param>
        public void Add(T value)
        {
            while (true)
            {
                var currentNodes = Volatile.Read(ref this.nodes);
                // Need to make sure it rereads the value each time.
                foreach (var node in currentNodes)
                {
                    if (node.TrySet(value))
                    {
                        return;
                    }
                }
                Thread.SpinWait(100);
                this.Grow();
            }
        }

        public bool Remove(Func<T, T, bool> compareFunc)
        {
            return false;
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
                var currentSize = Volatile.Read(ref this.nodes).Length;
                var newSize = currentSize * DOUBLE_UP_TO;
                if (newSize > DOUBLE_UP_TO)
                {
                    newSize = (int) Math.Floor(currentSize * GROWTH_FACTOR);
                }

                var newArray = new ArrayNode[newSize];
                for (var i = 0; i < newSize; i++)
                {
                    if (i < currentSize)
                    {
                        newArray[i] = this.nodes[i];
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

        /// <summary>
        ///     Called to shrink the data structure.
        /// </summary>
        private void Shrink()
        {
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
            ///     Clears a value at the position.
            /// </summary>
            /// <returns>true if the value was cleared.</returns>
            public bool Clear()
            {
                if (Interlocked.CompareExchange(
                        ref this.State,
                        CLEARING,
                        AVAILABLE) == CLEARING)
                {
                    Interlocked.Increment(ref this.Version);
                    Volatile.Write(ref this.Value, default);
                    Volatile.Write(ref this.State, AVAILABLE);
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
                           AVAILABLE) == AVAILABLE;
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