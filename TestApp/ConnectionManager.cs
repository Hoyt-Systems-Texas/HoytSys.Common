using System;
using A19.Messaging;
using A19.Messaging.Common;
using Microsoft.AspNetCore.Hosting.Internal;
using Mrh.Messaging;
using ServiceApplicationTester;

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

        public Guid RegisterConnection(string signalRConnection)
        {
            var connectionId = this.connectionIdGenerator.Generate();
            this.connections.AddOrUpdate(
                connectionId,
                signalRConnection);
            return connectionId;
        }
        
        
        public bool GetConnection(Guid connectionId, out ConnectionCollection<string>.ConnectionNode node)
        {
            return this.connections.GetConnection(connectionId, out node);
        }

        public void AddOrUpdate(Guid connectionId, string signalRConnection)
        {
            this.connections.AddOrUpdate(connectionId, signalRConnection);
        }
    }
}