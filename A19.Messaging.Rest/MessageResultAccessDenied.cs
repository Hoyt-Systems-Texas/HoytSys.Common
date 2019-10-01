namespace A19.Messaging.Rest
{
    public class MessageResultAccessDenied<T> : IMessageResult<T>
    {
        public MessageResultType ResultType => MessageResultType.AccessDenied;
    }
}