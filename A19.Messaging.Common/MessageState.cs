namespace A19.Messaging.Common
{
    public enum MessageState
    {
        New = 0,
        Persisted = 1,
        Running = 2,
        Completed = 100
    }
}