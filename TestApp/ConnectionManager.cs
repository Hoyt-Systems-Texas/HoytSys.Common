using System;
using A19.Messaging;
using A19.Messaging.Common;

namespace TestApp
{
    public class ConnectionManager
    {
        private readonly IConnectionIdGenerator connectionIdGenerator;

        private readonly ConnectionCollection<string> connections =
            new ConnectionCollection<string>(new TimeSpan(0, 1, 0));

        public ConnectionManager(
            IConnectionIdGenerator connectionIdGenerator)
        {
            this.connectionIdGenerator = connectionIdGenerator;
        }

        public Guid RegisterConnection(
            string signalRConnection,
            Guid userId)
        {
            var connectionId = this.connectionIdGenerator.Generate();
            this.connections.Add(
                connectionId,
                signalRConnection,
                userId);
            return connectionId;
        }
        
        
        public bool GetConnection(Guid connectionId, out ConnectionCollection<string>.ConnectionNode node)
        {
            return this.connections.GetConnection(connectionId, out node);
        }

        public void Update(Guid connectionId)
        {
            this.connections.Update(connectionId);
        }
    }
}