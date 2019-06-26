using System;
using System.Threading.Tasks;

namespace Mrh.Messaging
{
    public interface IMessageRouter<TPayloadType, TBody, TCtx> where TPayloadType:struct where TCtx:IMessageCtx<TPayloadType, TBody>
    {

        /// <summary>
        ///     Used to route a message to a handler.
        /// </summary>
        /// <param name="msgCtx">The message to route.</param>
        Task<MessageResult<TBody>> Route(TCtx msgCtx);

        /// <summary>
        ///     Used to register handlers on the message router.  Only call at started up.
        /// </summary>
        /// <param name="payloadType">The type of the payload.</param>
        /// <param name="accessFunc">The access function.</param>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        IMessageRouter<TPayloadType, TBody, TCtx> Register(
            TPayloadType payloadType,
            Func<TCtx, Task<bool>> accessFunc,
            Func<TCtx, Task<MessageResult<TBody>>> handler);
    }
}