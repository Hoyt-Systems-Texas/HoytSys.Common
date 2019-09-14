using A19.Concurrent.StateMachine;

namespace A19.StateMachine.PSharpBase.Distributed.ClientMachine.State
{
    public interface IClientHostAction : IAction<ClientHostState, ClientHostEvent, ClientStateMachineCtx, ClientHostParam>
    {
        
    }
}