using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Mrh.Concurrent;

namespace Mrh.Messaging
{
    
    /// <summary>
    ///     Used to collect the connection mapping.
    /// </summary>
    /// <typeparam name="TExternalConnection">The type for the external connection.</typeparam>
    public class ConnectionCollection<TExternalConnection> where TExternalConnection:IEquatable<TExternalConnection>
    {
        private readonly ConcurrentDictionary<string, ConnectionNode> connectionIdToExternal = new ConcurrentDictionary<string, ConnectionNode>(10, 1000);
        
        private readonly ConcurrentDictionary<TExternalConnection, ConnectionNode> externalToConnectionId = new ConcurrentDictionary<TExternalConnection, ConnectionNode>(10, 1000);

        private readonly long expiresAfterMs;

        private readonly StopWatchThreadSafe lastCleaned = new StopWatchThreadSafe();

        public ConnectionCollection(
            TimeSpan expiresAfter)
        {
            this.expiresAfterMs = (long) expiresAfter.TotalMilliseconds;
        }

        /// <summary>
        ///     Used to add or update an existing connection.
        /// </summary>
        /// <param name="connectionId">The internal connection identifier.</param>
        /// <param name="externalConnection">The external connection identifier.</param>
        public void AddOrUpdate(string connectionId, TExternalConnection externalConnection)
        {
            ConnectionNode node;
            if (this.connectionIdToExternal.TryGetValue(connectionId, out node))
            {
                node.LastSeen.Reset();
                if (!node.ExternalConnection.Equals(externalConnection))
                {
                    this.externalToConnectionId.TryRemove(externalConnection, out ConnectionNode ignore);
                    node = new ConnectionNode(
                        connectionId,
                        externalConnection);
                    this.externalToConnectionId.TryAdd(externalConnection, node);
                    this.connectionIdToExternal[connectionId] = node;
                }
            }
            else
            {
                node = new ConnectionNode(connectionId, externalConnection);
                if (this.connectionIdToExternal.TryAdd(connectionId, node))
                {
                    this.externalToConnectionId[externalConnection] = node;
                }
            }
        }

        /// <summary>
        ///     Used to get the connection information.
        /// </summary>
        /// <param name="connectionId">The id of the connection to get the information for.</param>
        /// <param name="node">The node with the connection information.</param>
        /// <returns>true if we where able to find the connection.</returns>
        public bool GetConnection(string connectionId, out ConnectionNode node)
        {
            return this.connectionIdToExternal.TryGetValue(connectionId, out node);
        }

        public struct ConnectionNode
        {
            public readonly string ConnectionId;
            public readonly TExternalConnection ExternalConnection;
            public readonly StopWatchThreadSafe LastSeen;

            public ConnectionNode(
                string connectionId,
                TExternalConnection externalConnection)
            {
                this.ConnectionId = connectionId;
                this.ExternalConnection = externalConnection;
                this.LastSeen = new StopWatchThreadSafe();
            }
        }
    }
}