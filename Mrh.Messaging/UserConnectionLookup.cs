using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mrh.Concurrent;

namespace Mrh.Messaging
{
    /// <summary>
    ///     Used to store connection in memory for fast lookup.  This class needs to be thread safe in all operations.  Needs to
    /// be non blocking to prevent queueing affects.
    /// </summary>
    public class UserConnectionLookup
    {
        private const int AVAILABLE = 0;
        private const int VALID = 1;
        private const int SETTING = 2;

        /// <summary>
        ///     The amount of time that has to go by before the request is expired.
        /// </summary>
        private readonly long expiredTimeMs;

        public UserConnectionLookup(
            long expiredTimeMs)
        {
            this.expiredTimeMs = expiredTimeMs;
        }

        /// <summary>
        ///     Storage on what connections belong to a user.  This is need for push notification.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, UserConnectionNode> userConnections =
            new ConcurrentDictionary<Guid, UserConnectionNode>(10, 1000);

        private readonly ConcurrentDictionary<string, ConnectionInfoNode> activeConnections =
            new ConcurrentDictionary<string, ConnectionInfoNode>(10, 1000);

        private struct ConnectionNode
        {
            public readonly string ConnectionId;

            public readonly Guid UserId;

            public readonly StopWatchThreadSafe LastSeen;

            public ConnectionNode(
                string connectionId,
                Guid userId)
            {
                this.ConnectionId = connectionId;
                this.UserId = userId;
                this.LastSeen = new StopWatchThreadSafe();
            }
        }

        /// <summary>
        ///     Used to add a user connection.
        /// </summary>
        /// <param name="connectionId">The id of the connection to add.</param>
        /// <param name="userId">The id of the user.</param>
        public void AddOrUpdateConnection(string connectionId, Guid userId)
        {
            ConnectionInfoNode node;
            if (this.activeConnections.TryGetValue(connectionId, out node))
            {
                node.LastSeen.Reset();
            }
            else
            {
                node = new ConnectionInfoNode(userId, connectionId);
                UserConnectionNode userNode;
                if (this.userConnections.TryGetValue(userId, out userNode))
                {
                    userNode.ConnectionIds.Add(node);
                }
                else
                {
                    userNode = new UserConnectionNode(userId);
                    userNode = this.userConnections.GetOrAdd(userId, userNode);
                    userNode.ConnectionIds.Add(node);
                }
            }
        }

        /// <summary>
        ///     Used to get the user's active connections.
        /// </summary>
        /// <param name="userId">The id of the user to get the connections for.</param>
        /// <returns>The id of the user.</returns>
        public IEnumerable<string> GetUsersConnections(Guid userId)
        {
            UserConnectionNode userNode;
            if (this.userConnections.TryGetValue(userId, out userNode))
            {
                return userNode.ConnectionIds.Select(v => v.ConnectionId);
            }
            return new string[0];
        }

        /// <summary>
        ///     Used to manage the user connections.
        /// </summary>
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

            public UserConnectionNode(
                Guid userId)
            {
                this.UserId = userId;
                this.ConnectionIds = new ConcurrentArrayBag<ConnectionInfoNode>(2);
            }
        }

        private class ConnectionInfoNode
        {
            public readonly Guid UserId;
            public readonly string ConnectionId;
            public readonly StopWatchThreadSafe LastSeen;

            public ConnectionInfoNode(
                Guid userId,
                string connectionId)
            {
                this.UserId = userId;
                this.ConnectionId = connectionId;
                this.LastSeen = new StopWatchThreadSafe();
            }
        }
    }
}