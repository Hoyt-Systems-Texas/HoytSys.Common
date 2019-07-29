namespace A19.StateMachine.PSharpBase
{
    public class EventActionNodePersist<TKey, TEvent, TParam, TUserId>
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
        ///     The parameter that was passed in.
        /// </summary>
        public TParam Param { get; set; }
        
        /// <summary>
        ///     The id of the user who made the request.
        /// </summary>
        public TUserId CreatedBy { get; set; }

    }
}