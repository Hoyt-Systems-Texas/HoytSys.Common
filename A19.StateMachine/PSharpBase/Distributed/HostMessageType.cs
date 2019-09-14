namespace A19.StateMachine.PSharpBase.Distributed
{
    public enum HostMessageType
    {
        Register = 1,
        RegisterAck = 2,
        Heartbeat = 3,
        HeartbeatAck = 4,
        ServerListRq = 5,
        ServerListRs = 6,
        AcquireStateMachine = 10,
        AcquireStateMachineAck = 11,
        ReleaseStateMachine = 12,
        ReleaseStateMachineAck = 13,
        RequestReleaseStateMachine = 14,
    }
}