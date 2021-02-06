using System;
using System.Collections.Concurrent;
using HoytSys.Core;

namespace A19.User.Common
{
    /// <summary>
    ///     Used to manage the sessions to different service.
    /// </summary>
    public class SystemSessionService : ISystemSessionService
    {
        private int _systemId;
        private readonly ISystemClient _systemClient;
        private readonly ConcurrentDictionary<int, SystemSessionNode> _systemSessionNodes = new ConcurrentDictionary<int, SystemSessionNode>();

        public SystemSessionService(
            ISystemClient systemClient,
            ISystemSettings systemSettings)
        {
            _systemClient = systemClient;
            _systemId = systemSettings.SystemId;
        }

        /// <summary>
        ///     Used to get a session for a specified system.
        /// </summary>
        /// <param name="accessingSystemId">The system id of the system you are trying to access.</param>
        /// <param name="passCode">The password to use for this system.</param>
        /// <param name="sessionId">The session id for the system.</param>
        /// <returns>true - if we successfully got the session.</returns>
        public bool TryGetSession(
            int accessingSystemId,
            string passCode,
            out string sessionId)
        {
            var systemSessionNode = GetSystemNode(accessingSystemId, passCode);
            return systemSessionNode.TryGetSessionId(out sessionId);
        }

        private SystemSessionNode GetSystemNode(int accessingSystemId, string passCode)
        {
            if (!_systemSessionNodes.TryGetValue(accessingSystemId, out var systemSessionNode))
            {
                systemSessionNode = new SystemSessionNode(
                    accessingSystemId,
                    passCode,
                    _systemClient,
                    _systemId);
                if (!_systemSessionNodes.TryAdd(accessingSystemId, systemSessionNode))
                {
                    while (!_systemSessionNodes.TryGetValue(accessingSystemId, out systemSessionNode)) ;
                }
            }

            return systemSessionNode;
        }

        /// <summary>
        ///     Used to get the observable for the session.
        /// </summary>
        /// <param name="accessSystemId">The id of the accessing system.</param>
        /// <param name="passCode">The pass code to use for that system.</param>
        /// <returns>The observable the emits the new session id when they change.</returns>
        public IObservable<string> GetSessionObservable(
            int accessSystemId,
            string passCode)
        {
            var systemNode = GetSystemNode(
                accessSystemId,
                passCode);
            return systemNode.SessionObservable;
        }
    }
}