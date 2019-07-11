using System;

namespace A19.StateMachine
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