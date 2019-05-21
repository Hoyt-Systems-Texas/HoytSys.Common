using System;
using System.Collections.Generic;

namespace Mrh.DataStructures
{
    /// <summary>
    ///     A simple binary heap that is array back.
    /// </summary>
    public class BinaryHeap<T>
    {
        private readonly IComparer<T> comparer;

        private T[] values;

        private int currentSize;

        public BinaryHeap(int size, IComparer<T> comp)
        {
            this.values = new T[size];
            this.currentSize = 0;
            this.comparer = comp;
        }

        public void Put(T value)
        {
            if (this.currentSize == this.values.Length - 1)
            {
                this.Resize();
            }

            this.values[this.currentSize++] = value;
            var previous = value;
            var p = this.currentSize;
            while (p > 1)
            {
                var nextP = p / 2;
                var current = this.values[nextP - 1];
                if (this.LessThan(previous, current))
                {
                    // Swap the values;
                    this.values[p - 1] = current;
                    this.values[nextP - 1] = previous;
                    p = nextP;
                }
                else
                {
                    break;
                }
            }
        }

        private bool LessThan(T previous, T current)
        {
            return this.comparer.Compare(previous, current) < 0;
        }

        private bool LessThanEqual(T previous, T current)
        {
            return this.comparer.Compare(previous, current) <= 0;
        }

        public bool Take(out T value)
        {
            if (this.currentSize > 0)
            {
                value = this.values[0];
                this.currentSize--;
                if (currentSize != 0)
                {
                    this.values[0] = this.values[this.currentSize];
                }

                this.values[this.currentSize] = default(T);
                this.PrecolatingDown();
                return true;
            }

            value = default(T);
            return false;
        }

        public bool VerifyHeap(int i)
        {
            if (2 * i + 2 >= this.currentSize)
            {
                return true;
            }

            bool left;
            if (this.LessThanEqual(this.values[i], this.values[2 * i + 1]))
            {
                left = VerifyHeap(2 * i + 1);
            }
            else
            {
                left = false;
            }

            bool right = this.LessThanEqual(this.values[i], this.values[2 * i + 2])
                         && VerifyHeap(2 * i + 2);
            return left && right;
        }

        private void PrecolatingDown()
        {
            if (this.currentSize <= 0)
            {
                return;
            }

            var k = 0;
            var tmp = this.values[k];
            var child = k;
            for (; (2 * k + 1) < this.currentSize; k = child)
            {
                child = (k * 2) + 1;
                if (child != this.currentSize - 1
                    && this.LessThan(this.values[child + 1], this.values[child]))
                {
                    child++;
                }

                if (this.LessThan(this.values[child], tmp))
                {
                    this.values[k] = this.values[child];
                }
                else
                {
                    break;
                }
            }

            this.values[k] = tmp;
        }

        private void Resize()
        {
            // Just double the size for now.
            var newSize = values.Length * 2;
            var newArray = new T[newSize];
            Array.Copy(
                this.values,
                0,
                newArray,
                0,
                this.values.Length);
            this.values = newArray;
        }
    }
}