using System.Collections.Generic;

namespace A19.Messaging.Rest
{
    public class MessageResultError<T> : IMessageResult<T>
    {
        public MessageResultType ResultType => MessageResultType.Error;
        
        public List<string> Errors { get; set; }
    }
}