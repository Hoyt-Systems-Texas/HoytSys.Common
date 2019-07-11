using A19.Core;
using A19.Messaging.Common;

namespace A19.Messaging
{
    public interface IIncomingMessageHandler<TPayloadType, TBody> : IStartable, IStoppable where TPayloadType : struct
    {
        /// <summary>
        ///     This call can't block for any reason and should use the build in thread pool for processing messages.
        /// </summary>
        /// <returns>true if we are able to accept the message.</returns>
        bool Handle(Message<TPayloadType, TBody> message);
    }
}