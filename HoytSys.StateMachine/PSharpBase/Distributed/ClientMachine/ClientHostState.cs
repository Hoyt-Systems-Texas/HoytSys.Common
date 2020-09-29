namespace A19.StateMachine.PSharpBase.Distributed.ClientMachine
{
    public enum ClientHostState
    {
        Pending = 0,
        Registering = 1,
        Registered = 2,
        StopStateMachines = 3,
        Reconnect = 4
    }
}