using System;

namespace HoytSys.DataStructures
{
    /// <summary>
    ///     A simple bit store used for compacting values.
    /// </summary>
    public class BitStore
    {
        private const int shift = 6;
        private const ulong posMask = 63;
        private ulong[] values;
        private readonly ulong mask;
        private readonly ulong bits;
        private readonly int length;

        public BitStore(
            int size,
            int bits)
        {
            this.mask = (ulong) (1 << bits) - 1;
            this.bits = (ulong) bits;
            this.length = (int) Math.Ceiling((bits * size) / 64.0);
            this.values = new ulong[this.length];
        }

        /// <summary>
        ///     Read the specific position in the block.
        /// </summary>
        /// <param name="pos">The position to read in.</param>
        /// <returns>The unit value at that position.</returns>
        public uint ReadUint(ulong pos)
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
            ulong shiftValue = (ulong) (end - start);
            valueMask = (shiftValue << shiftBits) - shiftValue;
            var endValue = this.values[end];
            endValue &= valueMask;
            var fix = ((int) this.bits - shiftBits) * (end - start);
            endValue = endValue << fix;
            value |= endValue;
            return (uint) value;
        }

        public void WriteUint(ulong pos, uint value)
        {
            var (start, reminder) = Start(pos);
            var longValue = (ulong) value;
            
            // Need the mask for the position we want to write to.
            var newMask = this.mask << reminder;
            // Zero out that position with an XOR.
            var zeroPosition = UInt64.MaxValue ^ newMask;
            var valueAtPos = this.values[start];
            // Shift the value to that position.
            longValue = longValue << reminder;
            // Zero out the position.
            valueAtPos &= zeroPosition;
            // Added our new value.
            valueAtPos |= longValue;
            this.values[start] = valueAtPos;
            
            // We will go through similar logic.
            longValue = value;
            var (end, shiftBits) = End(start, reminder);
            var shiftValue = (ulong) (end - start);
            var fix = ((int) this.bits - shiftBits);
            // Get the end position.
            newMask = (shiftValue << shiftBits) - shiftValue;
            // Get the zero mask.
            zeroPosition = UInt64.MaxValue ^ newMask;
            valueAtPos = this.values[end];
            longValue = longValue >> fix;
            valueAtPos &= zeroPosition;
            valueAtPos |= longValue;
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
            return ((int) (posInBits >> shift)
                ,(int) (posInBits & posMask));
        }

        /// <summary>
        ///     Calculates the ending position in the array.
        /// </summary>
        /// <returns>The ending position in the array.</returns>
        private (int, int) End(int start, int reminder)
        {
            var bitsU = (int) this.bits;
            var stopPosition = reminder + bitsU;
            var endReminder = stopPosition & 63;
            return (start + (stopPosition >> 6), endReminder);
        }
    }
}