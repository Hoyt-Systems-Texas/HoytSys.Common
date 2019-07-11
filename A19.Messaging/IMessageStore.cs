using System.Threading.Tasks;
using A19.Messaging.Common;

namespace A19.Messaging
{
    public interface IMessageStore<TPayloadType, TBody, TCtx> where TPayloadType:struct 
        where TCtx:IMessageCtx<TPayloadType, TBody>
    {

        /// <summary>
        ///     Stores a message to a permanent store.
        /// </summary>
        /// <param name="msgCtx">The message ctx to store.</param>
        /// <returns>The task that saves it.  If the message has already been processed then it return it instead.</returns>
        Task<TCtx> Save(TCtx msgCtx);

        /// <summary>
        ///     Used to try and get a msg by an id.
        /// </summary>
        /// <returns>A tuple where it's try if a value that was changed.</returns>
        Task<(bool, TCtx)> TryGet(MessageIdentifier identifier);

        /// <summary>
        ///     Used to store the result.
        /// </summary>
        /// <param name="result">The result of a message.</param>
        /// <returns>The message result.</returns>
        Task Save(MessageResult<TBody> result);

        /// <summary>
        ///     Used to get a message result by the identifier.
        /// </summary>
        /// <param name="identifier">The identifier for the message.</param>
        /// <returns>The result of lookup up the identifier.</returns>
        Task<(bool, MessageResult<TBody>)> TryGetResult(MessageIdentifier identifier);
    }
}