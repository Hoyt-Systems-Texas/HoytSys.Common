namespace A19.Messaging.Rest
{
    public interface IMessageResult<T>
    {
        MessageResultType ResultType { get; }
    }
}