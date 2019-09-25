using System.Security.Cryptography;

namespace A19.Core
{
    public class SecureKeyGenerator : ISecureKeyGenerator
    {
        private readonly RandomNumberGenerator _randomNumberGenerator = new RNGCryptoServiceProvider();

        /// <summary>
        ///     Used to generate a secure key.
        /// </summary>
        /// <param name="length">The length of the secure key to generate.</param>
        /// <returns></returns>
        public byte[] Generate(int length)
        {
            var key = new byte[length];
            _randomNumberGenerator.GetBytes(key);
            return key;
        }
    }
}