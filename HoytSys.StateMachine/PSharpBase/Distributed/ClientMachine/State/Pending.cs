using System.Collections.Generic;
using A19.Concurrent.StateMachine;

namespace A19.StateMachine.PSharpBase.Distributed.ClientMachine.State
{
    using EventActionType = EventActionTypeExt<ClientHostState, ClientHostEvent, ClientStateMachineCtx, ClientHostParam>;
    
    public sealed class Pending : BaseClientHostState
    {
        public override ClientHostState State => ClientHostState.Pending;

        public override IEnumerable<EventNode<ClientHostState, ClientHostEvent, ClientStateMachineCtx, ClientHostParam>>
            Events => new[]
            {
                EventActionType.GoTo(ClientHostEvent.Register, ClientHostState.Registered),
            };
    }
}