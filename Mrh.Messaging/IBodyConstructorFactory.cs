namespace Mrh.Messaging
{
    public interface IBodyConstructorFactory<TBody>
    {
        /// <summary>
        ///     Used to create a body reconstructor.
        /// </summary>
        /// <returns>The body constructor.</returns>
        IBodyReconstructor<TBody> Create();
    }
}