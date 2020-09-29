namespace A19.StateMachine.PSharpBase
{
    public enum EventResultType
    {
        Pending = 0,
        Retry = 1,
        ExitCompleted = 2,
        
        Completed = 10,
        Error = 11,
        Full = 12
    }
}