using System;

namespace HoytSys.Core
{
    public static class Pow2
    {
        public const int MAX_POW2 = (1 << 30);

        /// <summary>
        /// Finds the next positive value of 2.
        /// </summary>
        /// <param name="value">From which next positve power of two will be found.</param>
        /// <returns>The next power of 2, this value if it's a power of 2.</returns>
        public static int RoundToPowerOfTwo(int value)
        {
            if (value > MAX_POW2)
            {
                throw new ArgumentException($"There is no larger power of 2 int for values:{value} since it exceeds 2^31", nameof(value));
            }
            if (value < 0)
            {
                throw new ArgumentException($"Given value:{value}. Expecting value >= 0", nameof(value));
            }
            if (IsPowerOfTwo(value))
            {
                return value;
            }
            return NextPowerOf2(value);
        }

        /// <summary>
        /// Used to get the next power of 2.
        /// </summary>
        /// <param name="x">The number to get the next power of 2.</param>
        /// <returns>The next power of 2.</returns>
        public static int NextPowerOf2(int x)
        {
            x |= (x >> 1);
            x |= (x >> 2);
            x |= (x >> 4);
            x |= (x >> 8);
            x |= (x >> 16);
            return (x + 1);
        }

        /// <summary>
        ///     The minimum number of bits needed to represent a value.
        /// </summary>
        /// <param name="value">The value to represent.</param>
        /// <returns>The number of bits.</returns>
        public static int MinimumBits(int value)
        {
            var powOf2  = RoundToPowerOfTwo(value);
            return (int) Math.Log(powOf2, 2);
        }

        /// <summary>
        ///  Checks to see if the value is a power of 2
        /// </summary>
        /// <param name="value">The value to see if it's a power of 2</param>
        /// <returns>true if the value is a power of 2.</returns>
        public static bool IsPowerOfTwo(int value)
        {
            return (value & (value - 1)) == 0;
        }
       
    }
}