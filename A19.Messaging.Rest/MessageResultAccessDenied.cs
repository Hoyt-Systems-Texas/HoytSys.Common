namespace A19.Messaging.Rest
{
    public class MessageResultAccessDenied : IMessageResult
    {
        public MessageResultType ResultType => MessageResultType.AccessDenied;
    }
}