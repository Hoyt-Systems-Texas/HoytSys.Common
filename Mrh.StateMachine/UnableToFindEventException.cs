using System;

namespace Mrh.StateMachine
{
    public class UnableToFindEventException<TEvent> : Exception
    {
        public readonly TEvent TheEvent;

        public UnableToFindEventException(
            TEvent myEvent) : base($"Unable to find the event {myEvent}")
        {
            this.TheEvent = myEvent;
        }
    }
}