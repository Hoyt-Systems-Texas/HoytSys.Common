namespace A19.Core
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
    }
}