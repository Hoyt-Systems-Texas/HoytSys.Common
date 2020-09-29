using System;

namespace A19.StateMachine
{
    public class DuplicateFromToRegisteredException<TState> : Exception where TState: struct
    {

        public readonly TState FromState;

        public DuplicateFromToRegisteredException(
            TState from) : base($"The transition has already been register from {from}")
        {
            this.FromState = from;
        }
    }
}