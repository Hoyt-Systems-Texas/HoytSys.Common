using System;
using System.Text;

namespace HoytSys.Core
{
    public static class ByteArrayHelper
    {

        public static bool ByteArrayCompare(this byte[] b1, byte[] b2)
        {
            if (b1.Length == b2.Length)
            {
                for (var i = 0; i < b1.Length; i++)
                {
                    if ((b1[i] != b2[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static bool SpanByteCompare(this Span<byte> b1, Span<byte> b2)
        {
            if (b1.Length == b2.Length)
            {
                for (var i = 0; i < b1.Length; i++)
                {
                    if (b1[i] != b2[i])
                    {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }
    }
}