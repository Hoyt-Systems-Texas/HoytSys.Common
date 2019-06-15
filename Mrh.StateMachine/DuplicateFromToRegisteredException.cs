using System;

namespace Mrh.StateMachine
{
    public class DuplicateFromToRegisteredException<TState> : Exception where TState: struct
    {

        public readonly TState FromState;

        public readonly TState ToState;
        
        public DuplicateFromToRegisteredException(
            TState from,
            TState to) : base($"The transition has already been register from {from} to {to}")
        {
            this.FromState = from;
            this.ToState = to;
        }
    }
}