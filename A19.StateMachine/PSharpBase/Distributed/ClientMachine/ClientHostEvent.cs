namespace A19.StateMachine.PSharpBase.Distributed.ClientMachine
{
    public enum ClientHostEvent
    {
        Register = 0,
        RegistrationTimeout = 1,
        HeartbeatReceived = 2,
        SendMessage = 3,
        HeartbeatsFailed = 4,
        Reconnect = 5,
        ReconnectFailed = 6
    }
}