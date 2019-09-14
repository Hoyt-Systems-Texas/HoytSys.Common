namespace A19.StateMachine.PSharpBase.Distributed
{
    public enum HostStateMachineEvt
    {
        RequestingStateMachineEvt = 0,
        Acquire = 1,
        AcquireFailed = 2,
        AcquiredSuccess = 3,
        ReleaseRequested = 4,
        RequestDoneProcessing = 5,
        ReleasedTimeout = 6,
        ReleasedSuccess = 7,
        InactivityTimeout = 8,
        ReleasedInactivityTimeout = 9,
        
    }
}