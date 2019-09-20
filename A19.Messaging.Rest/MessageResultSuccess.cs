namespace A19.Messaging.Rest
{
    public sealed class MessageResultSuccess<T> : IMessageResult
    {

        public MessageResultType ResultType => MessageResultType.Success;
        
        public T Result { get; set; }
    }
}