using System.Threading.Tasks;

namespace Mrh.Messaging
{
    public interface IMessageRouter<TPayloadType, TBody> where TPayloadType:struct
    {

        /// <summary>
        ///     Used to route a message to a handler.
        /// </summary>
        /// <param name="msgCtx">The message to route.</param>
        Task<MessageResult<TBody>> Route(IMessageCtx<TPayloadType, TBody> msgCtx);
    }
}