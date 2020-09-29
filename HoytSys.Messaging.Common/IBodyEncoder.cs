namespace A19.Messaging.Common
{
    public interface IBodyEncoder<TBody>
    {
        /// <summary>
        ///     Used to decode a body response.
        /// </summary>
        /// <param name="body">The body to decode.</param>
        /// <typeparam name="T">The type you are decoding.</typeparam>
        /// <returns>The decoded type.</returns>
        T Decode<T>(TBody body);

        /// <summary>
        ///     Used to encode a body.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        /// <typeparam name="T">The type to encode.</typeparam>
        /// <returns>The body type.</returns>
        TBody Encode<T>(T value);
    }
}