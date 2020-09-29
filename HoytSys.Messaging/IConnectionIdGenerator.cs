using System;

namespace A19.Messaging
{
    public interface IConnectionIdGenerator
    {
        /// <summary>
        ///     Used to generate a unique connection id.  Make sure to use a secure random number generator when
        /// generating ids.
        /// </summary>
        /// <returns>The string of the id.</returns>
        Guid Generate();
    }
}