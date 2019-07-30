namespace A19.Concurrent.StateMachine
{
    public enum EventActionType
    {
        Do = 0,
        Goto = 1,
        Defer = 2, // Leave for now.  Not sure if it's worth implementing since a concurrent skip queue will be hard to write.
        Ignore = 3,
    }
}