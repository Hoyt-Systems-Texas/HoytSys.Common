using System;
using System.Collections.Concurrent;
using System.Threading;
using A19.Concurrent;

namespace A19.Messaging.Common
{
    /// <summary>
    ///     Used to collect the connection mapping.
    /// </summary>
    /// <typeparam name="TExternalConnection">The type for the external connection.</typeparam>
    public class ConnectionCollection<TExternalConnection> where TExternalConnection : IEquatable<TExternalConnection>
    {
        private const int IDLE = 0;
        private const int CLEANING = 1;

        private readonly ConcurrentDictionary<Guid, ConnectionNode> connectionIdToExternal =
            new ConcurrentDictionary<Guid, ConnectionNode>(10, 1000);

        private readonly ConcurrentDictionary<TExternalConnection, ConnectionNode> externalToConnectionId =
            new ConcurrentDictionary<TExternalConnection, ConnectionNode>(10, 1000);

        private readonly long expiresAfterMs;
        private readonly StopWatchThreadSafe lastCleaned = new StopWatchThreadSafe();
        private int state = IDLE;
        private readonly long cleanAfterMs = 60000;

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
        public void Add(Guid connectionId, TExternalConnection externalConnection, Guid userId)
        {
            if (this.lastCleaned.Elapsed() > this.cleanAfterMs)
            {
                this.Clean();
                this.lastCleaned.Reset();
            }
            ConnectionNode node;
            if (!this.connectionIdToExternal.ContainsKey(connectionId))
            {
                node = new ConnectionNode(
                    connectionId,
                    externalConnection, 
                    userId);
                if (this.connectionIdToExternal.TryAdd(connectionId, node))
                {
                    this.externalToConnectionId[externalConnection] = node;
                }
            }
        }

        public bool Update(Guid connectionId)
        {
            if (this.connectionIdToExternal.TryGetValue(connectionId, out var node))
            {
                node.LastSeen.Reset();
                return true;
            }
            else
            {
                return false;
            }
            
        }

        /// <summary>
        ///     Used to get the connection information.
        /// </summary>
        /// <param name="connectionId">The id of the connection to get the information for.</param>
        /// <param name="node">The node with the connection information.</param>
        /// <returns>true if we where able to find the connection.</returns>
        public bool GetConnection(Guid connectionId, out ConnectionNode node)
        {
            return this.connectionIdToExternal.TryGetValue(connectionId, out node);
        }

        private void Clean()
        {
            if (Interlocked.CompareExchange(
                    ref this.state,
                    CLEANING,
                    IDLE) == IDLE)
            {
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    try
                    {
                        foreach (var node in this.connectionIdToExternal.Values)
                        {
                            if (node.LastSeen.Elapsed() > this.expiresAfterMs)
                            {
                                this.connectionIdToExternal.TryRemove(
                                    node.ConnectionId,
                                    out ConnectionNode ignore);
                                this.externalToConnectionId.TryRemove(
                                    node.ExternalConnection,
                                    out ConnectionNode ignore1);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        Interlocked.Exchange(ref this.state, IDLE);
                    }
                });
            }
        }

        public struct ConnectionNode
        {
            public readonly Guid ConnectionId;
            public readonly TExternalConnection ExternalConnection;
            public readonly StopWatchThreadSafe LastSeen;
            public readonly Guid userId;

            public ConnectionNode(
                Guid connectionId,
                TExternalConnection externalConnection,
                Guid userId)
            {
                this.ConnectionId = connectionId;
                this.ExternalConnection = externalConnection;
                this.LastSeen = new StopWatchThreadSafe();
                this.userId = userId;
            }
        }
    }
}