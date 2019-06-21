using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Mrh.Concurrent;

namespace Mrh.Messaging
{
    /// <summary>
    ///     Used to store connection in memory for fast lookup.  This class needs to be thread safe in all operations.  Needs to
    /// be non blocking to prevent queueing affects.
    /// </summary>
    /// <typeparam name="TInternalConnection">The type for the internal connections.  This code be anything from a string,
    /// a GUID, URI, etc</typeparam>
    public class ConnectionLookup<TInternalConnection>
    {

        private const int AVAILABLE = 0;
        private const int VALID = 1;
        private const int SETTING = 2;

        /// <summary>
        ///     The amount of time that has to go by before the request is expired.
        /// </summary>
        private readonly long expiredTimeMs;

        public ConnectionLookup(
            long expiredTimeMs)
        {
            this.expiredTimeMs = expiredTimeMs;
        }

        /// <summary>
        ///     The look for internal connections.
        /// </summary>
        private readonly ConcurrentDictionary<TInternalConnection, ConnectionNode> internalConnections;

        /// <summary>
        ///     The lookup for the application specific connection id.  This is what's used to route the messages.
        /// </summary>
        private readonly ConcurrentDictionary<string, ConnectionNode> appConnections;

        /// <summary>
        ///     Storage on what connections belong to a user.  This is need for push notification.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, UserConnectionNode> userConnections;
        
        private struct ConnectionNode
        {
            public readonly string ConnectionId;

            public readonly TInternalConnection InternalConnectionId;

            public readonly Guid UserId;

            public readonly StopWatchThreadSafe LastSeen;

            public ConnectionNode(
                string connectionId,
                TInternalConnection internalConnectionId,
                Guid userId)
            {
                this.ConnectionId = connectionId;
                this.InternalConnectionId = internalConnectionId;
                this.UserId = userId;
                this.LastSeen = new StopWatchThreadSafe();
            }
        }

        private struct UserConnectionNode
        {
            /// <summary>
            ///     The id of a user.
            /// </summary>
            public readonly Guid UserId;

            /// <summary>
            ///     This list of the user connection ids.
            /// </summary>
            public ConcurrentArrayBag<ConnectionInfoNode> ConnectionIds;

            /// <summary>
            ///     The amount of time that has gone by the last time this user has been seen.
            /// </summary>
            public StopWatchThreadSafe LastSeen;

            public UserConnectionNode(
                Guid userId)
            {
                this.UserId = userId;
                this.LastSeen = new StopWatchThreadSafe();
                this.ConnectionIds = new ConcurrentArrayBag<ConnectionInfoNode>(2);
            }
        }

        private class ConnectionInfoNode
        {
        }
    }
}