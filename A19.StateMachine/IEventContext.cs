namespace A19.StateMachine
{
    public interface IEventContext<TState> where TState:struct
    {
        TState CurrentState { get; }
    }
}