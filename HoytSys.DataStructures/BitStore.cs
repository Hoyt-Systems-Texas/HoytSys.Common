using System;

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
            this.length = (int) Math.Ceiling((bits * size) / 64.0);
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
            var start = 0ul;
            var middle = searchLength / 2;
            var end = searchLength;
            var pos = middle;
            while (true)
            {
                var valueAtPos = Read(pos * increments);
                if (value > valueAtPos)
                {
                    start = pos + 1;
                    pos = CalculateHigh(start, end);
                    if (pos >= end)
                    {
                        break;
                    }
                } 
                else if (value < valueAtPos)
                {
                    // Value low so we know it must be less than this.
                    end = pos - 1; // Subtract one off of it.
                    pos = CalculateLow(start, end);
                    if (pos <= start)
                    {
                        break;
                    }
                }
                else
                {
                    // Value is equal.  We keep doing so we can support duplicate values.
                    end = pos;
                    pos = CalculateLow(start, end);
                    if (pos >= end)
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

        private ulong CalculateLow(ulong start, ulong end)
        {
            var range = end - start;
            var middle = range / 2;
            return middle + start;
        }

        private ulong CalculateHigh(ulong start, ulong end)
        {
            var range = end - start;
            var middle = range / 2;
            return middle + start;
        }

        /// <summary>
        ///     Read the specific position in the block. No branching at in reed for speed.  The reason it's so much faster is due to the fact branch misses are really expensive so by eliminating them the code runs much faster.
        /// </summary>
        /// <param name="pos">The position to read in.</param>
        /// <returns>The unit value at that position.</returns>
        public ulong Read(ulong pos)
        {
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
            // the hasReminder can either be 1 or 0 and we can use this fact to wipe out a
            // value using multiplication and shift with a starting bit set since it's 1.
            ulong hasReminderUl = (ulong) (end - start);
            int hasReminder = (end - start);
            // Creates the mask if there is a reminder.  Reminder is 1 when we want to modify
            // it so we can use that fact to create the mask.
            // Doing a shift of 0 is zero so this works just fine.
            valueMask = (hasReminderUl << shiftBits) - hasReminderUl;
            // Gets the ending value.
            var endValue = this.values[end];
            // Get the value here using the mask. when there isn't a reminder this will be 0.
            endValue &= valueMask;
            // gets the position to shift the value to.  we we don't want to do that we use the fact when end-start is equal it's 0 so we can get fix of 0.
            var fix = (this.bitsI - shiftBits) * hasReminder;
            endValue = endValue << fix;
            // Add the reminder value onto the value.
            value |= endValue;
            return value;
        }

        /// <summary>
        ///     Writes a value to a position.  Doesn't have any branching to improve speed.
        /// </summary>
        /// <param name="pos">The position to write the value to.</param>
        /// <param name="value">The value to write.</param>
        public void Write(ulong pos, ulong value)
        {
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
            // A trick to get 0 if we don't want to update the value.  This will either be 1 or 0.
            var hasReminder = (ulong) (end - start);
            // The reminder bits to update.
            var fix = (this.bitsI - shiftBits);
            // Get the end position. zero out the value if start matches end.
            // If update value is 0 then it will create a mask of all 0s.
            newMask = (hasReminder << shiftBits) - hasReminder;
            // Uses and xor with the new mask to 0 out the values since the mask will all be 1s at the position.
            // When the update value is 0 the new masks is all 0s so nothing gets updated.
            zeroPosition = UInt64.MaxValue ^ newMask;
            valueAtPos = this.values[end];
            // If we are at the start/end zero out the value.
            // If we don't want to overwrite the longValue is now 0.
            longValue = (longValue >> fix) * hasReminder;
            // Use logical and to zero those series of bytes with the ones we are about to write are 0.
            // if update value is 0 then zeroPosition is all ones so value at position doesn't change.
            valueAtPos &= zeroPosition;
            // Use logical or to update just those bytes.
            // when update value is 0 long value is 0 so the value remains unchanged.
            valueAtPos |= longValue;
            // Update the value with the new one.
            this.values[end] = valueAtPos;
        }

        /// <summary>
        ///     Used to calculate the start index in the array.
        /// </summary>
        /// <param name="pos">The to get in the bit store.</param>
        /// <returns>The starting index of the value in the bit vector.</returns>
        private (int, int) Start(ulong pos)
        {
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