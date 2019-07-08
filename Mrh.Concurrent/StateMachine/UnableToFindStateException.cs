using System;

namespace Mrh.Concurrent.StateMachine
{
    public class UnableToFindStateException<TState> : Exception
        where TState: struct
    {
        public readonly TState State;

        public UnableToFindStateException(
            TState state) : base($"Unable to find state exception {state}")
        {
            this.State = State;
        }
    }
}