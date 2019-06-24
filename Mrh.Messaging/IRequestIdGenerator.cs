namespace Mrh.Messaging
{
    public interface IRequestIdGenerator
    {
        /// <summary>
        ///     Used to generate the next id.
        /// </summary>
        /// <returns>The id for the message.</returns>
        long Next();
    }
}