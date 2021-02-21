using System;
using System.Diagnostics;

namespace HoytSys.DataStructures
{
    /// <summary>
    ///     A simple bit store used for compacting values.  The goal is to have a simple structure
    /// to minimize memory needed but it's not as efficient as compact data structures.
    /// </summary>
    public class BitStore
    {
        private const int shift = 6;
        private const ulong posMask = 63;
        private ulong[] values;
        private readonly ulong mask;
        private readonly ulong bits;
        private readonly int bitsI;
        private readonly int length;
        private readonly int size;
        private readonly ulong sizeL;

        public BitStore(
            int size,
            int bits)
        {
            this.mask = (ulong) (1 << bits) - 1;
            this.bits = (ulong) bits;
            this.bitsI = bits;
            this.length = ((bits * size) / 64) + 1;
            this.values = new ulong[this.length];
            this.size = size;
            this.sizeL = (ulong) size;
        }

        /// <summary>
        ///     Used to create an empty bit store with the same bits
        /// </summary>
        /// <returns>A new bit vector using the same number of bits with the new specified size.</returns>
        public BitStore CreateNew(int newSize)
        {
            return new BitStore(newSize, this.bitsI);
        }

        /// <summary>
        ///     Create a new bit bitvector with the new specified size.
        /// </summary>
        /// <param name="newSize">The neww size of the vector.  Must be at least as big as the current bit vector.</param>
        /// <returns>The new bit store.</returns>
        /// <exception cref="ArgumentException"></exception>
        public BitStore Clone(int newSize)
        {
            if (newSize < this.size)
            {
                throw new ArgumentException($"Must be at least the same size as the current vector", nameof(newSize));
            }
            else
            {
                var vector = new BitStore(newSize, this.bitsI);
                for (var i = 0; i < this.length; i++)
                {
                    vector.values[i] = this.values[i];
                }
                return vector;
            }
        }

        public int Count => this.size;

        /// <summary>
        ///     A binary search for the bit store.  It assume the store is in order.
        /// </summary>
        /// <param name="value">The value we are looking for.</param>
        /// <param name="increments">The increments to use to search for.</param>
        /// <returns>The position of the value start. If not found returns UInt64.MaxValue</returns>
        public ulong BinarySearch(ulong value, ulong increments)
        {
            // Divided by the increments the total length.  Don't allow to overflow!
            var searchLength =  this.sizeL / increments;
            var startIdx = 0ul;
            var middleIdx = searchLength / 2;
            var endIdx = searchLength - 1;
            var pos = middleIdx;
            while (true)
            {
                var valueAtPos = Read(pos * increments);
                if (value > valueAtPos)
                {
                    startIdx = pos + 1;
                    pos = CalculateMiddle(startIdx, endIdx);
                    if (pos >= endIdx)
                    {
                        break;
                    }
                } 
                else if (value < valueAtPos)
                {
                    // Value low so we know it must be less than this.
                    endIdx = pos - 1; // Subtract one off of it.
                    pos = CalculateMiddle(startIdx, endIdx);
                    if (pos <= startIdx)
                    {
                        break;
                    }
                }
                else
                {
                    // Value is equal.  We keep doing so we can support duplicate values.
                    endIdx = pos;
                    pos = CalculateMiddle(startIdx, endIdx);
                    if (pos >= endIdx)
                    {
                        break;
                    }
                }
            }

            var matchedValue = Read(pos * increments);
            if (matchedValue == value)
            {
                return pos * increments;
            }
            else
            {
                return UInt64.MaxValue;
            }
        }

        private ulong CalculateMiddle(ulong start, ulong end)
        {
            Debug.Assert(start < this.sizeL);
            Debug.Assert(end < this.sizeL);
            var range = end - start;
            var middle = range / 2;
            return middle + start;
        }

        /// <summary>
        ///     Read the specific position in the block. No branching at in reed for speed
        /// </summary>
        /// <param name="pos">The position to read in.</param>
        /// <returns>The unit value at that position.</returns>
        public ulong Read(ulong pos)
        {
            Debug.Assert(pos < this.sizeL);
            var (start, reminder) = Start(pos);
            
            // All we need to do here is shift the mask to the correct position.
            var valueMask = this.mask << reminder;
            // Now we take the mask of the array.
            var value = this.values[start];
            // Get the bits at that position.
            value &= valueMask;
            // Shift it to the start.
            value = value >> reminder;
            
            var (end, shiftBits) = End(start, reminder);
            if (end == start)
            {
                return value;
            }
            else
            {
                // Creates the mask if there is a reminder.
                valueMask = (1ul << shiftBits) - 1ul;
                // Gets the ending value.
                var endValue = this.values[end];
                // Get the value here using the mask. when there isn't a reminder this will be 0.
                endValue &= valueMask;
                // gets the position to shift the value to.
                var fix = (this.bitsI - shiftBits);
                endValue = endValue << fix;
                // Add the reminder value onto the value.
                value |= endValue;
                return value;
            }
        }

        /// <summary>
        ///     Writes a value to a position.  Doesn't have any branching to improve speed.
        /// </summary>
        /// <param name="pos">The position to write the value to.</param>
        /// <param name="value">The value to write.</param>
        public void Write(ulong pos, ulong value)
        {
            Debug.Assert(pos < this.sizeL);
            var (start, reminder) = Start(pos);
            
            // Need the mask for the position we want to write to.
            var newMask = this.mask << reminder;
            // Zero out that position with an XOR.
            var zeroPosition = UInt64.MaxValue ^ newMask;
            var valueAtPos = this.values[start];
            // Shift the value to that position.
            var longValue = value << reminder;
            // Zero out the position.
            valueAtPos &= zeroPosition;
            // Added our new value.
            valueAtPos |= longValue;
            this.values[start] = valueAtPos;
            
            // We will go through similar logic.
            longValue = value;
            var (end, shiftBits) = End(start, reminder);
            if (start == end)
            {
                // They match do nothing.
            }
            else
            {
                // The reminder bits to update.
                var fix = (this.bitsI - shiftBits);
                // Get the end position. zero out the value if start matches end.
                newMask = (1ul << shiftBits) - 1ul;
                // Uses and xor with the new mask to 0 out the values since the mask will all be 1s at the position.
                zeroPosition = UInt64.MaxValue ^ newMask;
                valueAtPos = this.values[end];
                // If we are at the start/end zero out the value.
                longValue = (longValue >> fix);
                // Use logical and to zero those series of bytes with the ones we are about to write are 0.
                valueAtPos &= zeroPosition;
                // Use logical or to update just those bytes.
                valueAtPos |= longValue;
                // Update the value with the new one.
                this.values[end] = valueAtPos;
            }
        }

        /// <summary>
        ///     Used to calculate the start index in the array.
        /// </summary>
        /// <param name="pos">The to get in the bit store.</param>
        /// <returns>The starting index of the value in the bit vector.</returns>
        private (int, int) Start(ulong pos)
        {
            Debug.Assert(pos < this.sizeL);
            // We need at leas a long here since it could easily overflow on a large array.
            var posInBits = pos * bits;
            // Get the starting index by shifting by 6 to cut of the reminder.
            var startIdx = (int) (posInBits >> shift);
            // Calculating starting off offset by get the reminder using an logical and of the mask.
            var startOffset = (int) (posInBits & posMask);
            return (startIdx
                ,startOffset);
        }

        /// <summary>
        ///     Calculates the ending position in the array.
        /// </summary>
        /// <returns>The ending position in the array.</returns>
        private (int, int) End(int start, int reminder)
        {
            Debug.Assert(start < this.size);
            // If it overflows to the ulong it will set the 7 bit to 1 and we just
            // need to shift 6 to get the value.
            var stopPosition = reminder + this.bitsI;
            // Will get the reminder if it overflows into the next ulong.
            var endReminder = stopPosition & 63;
            // A trick to see if we should add one.
            var stop = start + (stopPosition >> 6);
            return (stop, endReminder);
        }
    }
}