using System.Collections.Generic;
using System.Threading.Tasks;
using A19.Concurrent.StateMachine;

namespace A19.StateMachine.PSharpBase.Distributed.ClientMachine.State
{
    using EventActionType = EventActionTypeExt<ClientHostState, ClientHostEvent, ClientStateMachineCtx, ClientHostParam>;
    
    public class Registering : BaseClientHostState
    {
        public override Task Entry(ClientHostEvent @event, ClientStateMachineCtx ctx, ClientHostParam param)
        {
            return Task.FromResult(0);
        }

        public override ClientHostState State => ClientHostState.Registering;

        public override IEnumerable<EventNode<ClientHostState, ClientHostEvent, ClientStateMachineCtx, ClientHostParam>>
            Events
            => new[]
            {
                EventActionType.GoTo(ClientHostEvent.RegistrationTimeout, ClientHostState.Registering)
            };
    }
}