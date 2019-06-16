namespace Mrh.StateMachine
{
    public interface IEventContext<TState> where TState:struct
    {
        TState CurrentState { get; }
    }
}