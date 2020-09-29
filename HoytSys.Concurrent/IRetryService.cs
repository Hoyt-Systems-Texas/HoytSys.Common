using System;

namespace A19.Concurrent
{
    
    /// <summary>
    ///     A retry service meant for  asynchronous events.
    /// </summary>
    public interface IRetryService
    {
        /// <summary>
        ///     Used to start the retry service.
        /// </summary>
        void Start();

        /// <summary>
        ///     Used to stop the retry service.
        /// </summary>
        void Stop();

        /// <summary>
        ///     Action to be retried after so much time has gone by.
        /// </summary>
        /// <param name="retry">The amount of time that goes by for retrying.</param>
        /// <param name="act">The action to run when the timeout expires.</param>
        void Retry(TimeSpan retry, Action act);

    }
}