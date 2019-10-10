using System;

namespace A19.Core
{
    public interface ISecureKeyGenerator
    {
        /// <summary>
        ///     Used to generate a secure key.
        /// </summary>
        /// <param name="length">The length of the secure key to generate.</param>
        /// <returns></returns>
        byte[] Generate(int length);

        Guid GenerateGuid();
        void Generate(Span<byte> span);
    }
}