using System;
using System.Security.Cryptography;

namespace HoytSys.Core
{
    public static class SecurityHelpers
    {
        /// <summary>
        ///     Used to compute a Sha512 hash.
        /// </summary>
        /// <param name="value">The value to hash</param>
        /// <returns>The hash valued.</returns>
        public static byte[] Sha512(byte[] value)
        {
            using (var hash = SHA512.Create())
            {
                return hash.ComputeHash(value);
            }
        }

        public static Span<byte> Hmac512(
            byte[] key,
            byte[] value,
            int start,
            int count)
        {
            using (var hmac = new HMACSHA512(key))
            {
                return hmac.ComputeHash(value, start, count);
            }
        }
    }
}