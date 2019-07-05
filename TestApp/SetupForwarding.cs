using System;
using Microsoft.AspNetCore.SignalR;
using Mrh.Messaging.Client;
using ServiceApplicationTester;
using TestApp.Hubs;

namespace TestApp
{
    public class SetupForwarding
    {
        private readonly IForwardingClient<PayloadType, string> forwardingClient;
        private readonly ConnectionManager connectionManager;
        private readonly IHubContext<BrowserHub, BrowserHub.IBrowserClient> hubContext;

        public SetupForwarding(
            IHubContext<BrowserHub, BrowserHub.IBrowserClient> hubContext,
            IForwardingClient<PayloadType, string> forwardingClient,
            ConnectionManager connectionManager)
        {
            this.hubContext = hubContext;
            this.forwardingClient = forwardingClient;
            this.forwardingClient.Start();
            this.connectionManager = connectionManager;
            
            this.Configure();
        }

        private void Configure()
        {
            this.forwardingClient.Start();
            this.forwardingClient.Receive.Subscribe(env =>
            {
                if (this.connectionManager.GetConnection(env.ConnectionId, out var node))
                {
                    hubContext.Clients.Client(node.ExternalConnection).Received(
                        env);
                }
            });
        }
    }
}