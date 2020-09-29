namespace A19.StateMachine.PSharpBase.Distributed
{
    public enum HostStateMachineState
    {
        Standby = 0,
        Requested = 1,
        Acquiring = 2,
        Acquired = 3,
        ReleaseRequested = 4,
        Releasing = 5,
        ReleasingInactive = 6,
        Removed = 7,
        ServerListRequest = 8
    }
}