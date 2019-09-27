using System.Security.Cryptography;

namespace A19.Core
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
    }
}