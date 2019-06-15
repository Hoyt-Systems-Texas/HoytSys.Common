namespace Mrh.StateMachine
{
    public struct TransitionResult<TEvent>
    {
        public readonly TEvent Event;

        public readonly TransitionResultType ResultType;
        
        public TransitionResult()                                                                         
    }
}