using System;

namespace A19.User.Common
{
    public interface ISystemSessionService
    {
        /// <summary>
        ///     Used to get the observable for the session.
        /// </summary>
        /// <param name="accessSystemId">The id of the accessing system.</param>
        /// <param name="passCode">The pass code to use for that system.</param>
        /// <returns>The observable the emits the new session id when they change.</returns>
        IObservable<string> GetSessionObservable(
            int accessSystemId,
            string passCode);
    }
}