namespace A19.StateMachine.PSharpBase
{
    public class EventActionNodePersist<TKey, TEvent, TParam>
    {
        /// <summary>
        ///     The id of the event.
        /// </summary>
        public readonly long Id;

        /// <summary>
        ///     The key for the state machine.
        /// </summary>
        public readonly TKey StateMachineKey;

        /// <summary>
        ///     The event node.
        /// </summary>
        public readonly TEvent Event;

        /// <summary>
        ///     The parameter that was passed in.
        /// </summary>
        public readonly TParam Param;
        
        public EventActionNodePersist(
            long id,
            TKey stateMachineKey,
            TEvent @event,
            TParam param)
        {
            this.Id = id;
            this.StateMachineKey = stateMachineKey;
            this.Event = @event;
            this.Param = param;
        }

    }
}