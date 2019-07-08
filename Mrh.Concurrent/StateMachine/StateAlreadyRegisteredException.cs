using System;

namespace Mrh.Concurrent.StateMachine
{
    public class StateAlreadyRegisteredException<TState> : Exception
        where TState: struct

    {

        public readonly TState State;
        
        public StateAlreadyRegisteredException(TState state) : base($"The state {state} has already been registered.")
        {
            this.State = state;
        }
    }
}