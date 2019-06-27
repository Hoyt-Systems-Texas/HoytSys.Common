using System;
using System.Security.Cryptography;

namespace Mrh.Messaging
{
    /// <summary>
    ///     Used to generate unique connections id that are safe to be passed around.
    /// </summary>
    public class ConnectionIdGenerator : IConnectionIdGenerator
    {
        
        private static readonly RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();

        public Guid Generate()
        {
            var id = new byte[16];
            random.GetBytes(id);
            return new Guid(id);
        }
    }
}