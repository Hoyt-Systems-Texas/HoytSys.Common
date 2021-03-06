using System;

namespace A19.StateMachine
{
    public class UnableToFindStateException<TState> : Exception where TState:struct
    {

        public readonly TState State;
        
        public UnableToFindStateException(
            TState state) : base($"Unable to find the object with state {state}")
        {
            this.State = state;
        }
    }
}