using System;
using System.Threading.Tasks;

namespace A19.StateMachine.PSharpBase
{
    public class EventActionNodePersist<TKey, TState, TEvent, TParam, TCtx, TUserId>
        where TState: struct
        where TEvent: struct
        where TCtx: AbstractStateMachinePersistCtx<TKey, TState, TEvent, TParam, TCtx, TUserId>
    {
        
        /// <summary>
        ///     The id of the event.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     The key for the state machine.
        /// </summary>
        public TKey StateMachineKey { get; set; }

        /// <summary>
        ///     The event node.
        /// </summary>
        public TEvent Event { get; set; }
        
        /// <summary>
        ///     Indicates this should just run on the state machine thread.
        /// </summary>
        public bool RunOnThread { get; set; }
        
        /// <summary>
        ///     The function to run on the main thread.
        /// </summary>
        public Func<TCtx, Task> FuncToRun { get; set; }

        /// <summary>
        ///     The parameter that was passed in.
        /// </summary>
        public TParam Param { get; set; }
        
        /// <summary>
        ///     The id of the user who made the request.
        /// </summary>
        public TUserId CreatedBy { get; set; }
        
        /// <summary>
        ///     The results of the action.
        /// </summary>
        public EventResultType EventResult { get; set; }
        
    }
}