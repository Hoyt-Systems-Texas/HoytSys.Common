using System.Collections.Generic;

namespace Mrh.Messaging.Json
{
    public class JsonMessageResultFactory : IMessageResultFactory<string>
    {
        public MessageResult<string> CreateError(List<string> errors)
        {
            return new MessageResult<string>
            {
                Result = MessageResultType.Error,
                Body = JsonHelper.Encode(errors)
            };
        }

        public MessageResult<string> CreateBusy()
        {
            return new MessageResult<string>
            {
                Result = MessageResultType.Busy
            };
        }

        public MessageResult<string> AccessDenied()
        {
            return new MessageResult<string>
            {
                Result = MessageResultType.AccessDenied,
                Body = JsonHelper.Encode(new List<string>
                {
                    "Access Denied"
                })
            };
        }

        public MessageResult<string> State(MessageState messageState)
        {
            return new MessageResult<string>
            {
                Result = MessageResultType.Success,
                Body = JsonHelper.Encode(messageState)
            };
        }
    }
}