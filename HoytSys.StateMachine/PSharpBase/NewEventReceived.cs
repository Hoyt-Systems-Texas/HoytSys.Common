using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase
{
    public class NewEventReceived<TKey, TState, TEvent, TCtx, TParam, TUserId> 
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {

        private readonly StateMachinePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId> stateMachinePersisted;
        
        public NewEventReceived(
            StateMachinePersisted<TKey, TState, TEvent, TCtx, TParam, TUserId> stateMachinePersisted)
        {
            this.stateMachinePersisted = stateMachinePersisted;
        }

        public async Task NewEvent(TCtx stateMachinePersistCtx)
        {
            await this.stateMachinePersisted.HandleTransition(stateMachinePersistCtx);
        }
    }
}