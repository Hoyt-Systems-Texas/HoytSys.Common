using System.Threading.Tasks;

namespace Mrh.Messaging
{
    public interface IMessageStore<TPayloadType, TBody> where TPayloadType:struct
    {

        /// <summary>
        ///     Stores a message to a permanent store.
        /// </summary>
        /// <param name="msgCtx">The message ctx to store.</param>
        /// <returns>The task that saves it.  If the message has already been processed then it return it instead.</returns>
        Task<IMessageCtx<TPayloadType, TBody>> Save(IMessageCtx<TPayloadType, TBody> msgCtx);

        /// <summary>
        ///     Used to try and get a msg by an id.
        /// </summary>
        /// <returns>A tuple where it's try if a value that was changed.</returns>
        Task<(bool, IMessageCtx<TPayloadType, TBody>)> TryGet();
    }
}