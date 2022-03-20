using System;
using System.Security.Cryptography;

namespace HoytSys.Core
{
    public class SecureKeyGenerator : ISecureKeyGenerator
    {
        private readonly RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

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

        public void Generate(Span<byte> span)
        {
            _randomNumberGenerator.GetBytes(span);
        }

        public Guid GenerateGuid()
        {
            var key = new byte[16];
            _randomNumberGenerator.GetBytes(key);
            return new Guid(key);
        }
    }
}