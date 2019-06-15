using System;

namespace Mrh.StateMachine
{
    public class DuplicateStateRegisterException<TState> : Exception
    {

        public readonly TState State;
        
        public DuplicateStateRegisterException(TState state) : base($"The state {state} has already been registered.")
        {
            this.State = state;
        }
    }
}