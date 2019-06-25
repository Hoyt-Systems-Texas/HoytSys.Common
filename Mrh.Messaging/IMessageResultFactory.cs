using System.Collections.Generic;
using NLog.MessageTemplates;

namespace Mrh.Messaging
{
    public interface IMessageResultFactory<TBody>
    {

        /// <summary>
        ///     Used to create an error message result.
        /// </summary>
        /// <param name="errors">The list of errors.</param>
        /// <returns>The message result.</returns>
        MessageResult<TBody> CreateError(List<string> errors);

        /// <summary>
        ///     Creates a busy result.
        /// </summary>
        /// <returns>The message result.</returns>
        MessageResult<TBody> CreateBusy();

        /// <summary>
        ///     Creates an access denied result.
        /// </summary>
        /// <returns></returns>
        MessageResult<TBody> AccessDenied();

        /// <summary>
        ///     Used to create a message state response.
        /// </summary>
        /// <param name="messageState">The current message state.</param>
        /// <returns>The message result.</returns>
        MessageResult<TBody> State(MessageState messageState);
    }
}