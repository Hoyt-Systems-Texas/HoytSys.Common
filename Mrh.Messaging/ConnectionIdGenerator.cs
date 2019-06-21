using System;
using System.Security.Cryptography;

namespace Mrh.Messaging
{
    public class ConnectionIdGenerator : IConnectionIdGenerator
    {
        
        private static readonly RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();

        private readonly int length;

        /// <summary>
        ///     Used to create a new id generator.
        /// </summary>
        /// <param name="length">The length of the generator key in bytes.</param>
        public ConnectionIdGenerator(
            int length)
        {
            this.length = length;
        }
        
        public string Generate()
        {
            var id = new byte[this.length];
            random.GetBytes(id);
            return Convert.ToBase64String(id);
        }
    }
}