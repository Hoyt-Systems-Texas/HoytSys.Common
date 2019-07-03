using System;
using System.Threading.Tasks;
using A19.Security.User;
using Microsoft.AspNetCore.SignalR;
using Mrh.Messaging;
using Mrh.Messaging.Client;
using Mrh.Messaging.Common;
using ServiceApplicationTester;

namespace TestApp.Hubs
{
    public class BrowserHub : Hub<BrowserHub.IBrowserClient>
    {
        private readonly IForwardingClient<PayloadType, string> forwardingClient;
        private readonly IConnectionIdGenerator connectionIdGenerator;

        private readonly ConnectionCollection<string> connections =
            new ConnectionCollection<string>(new TimeSpan(0, 1, 0));

        private readonly IUserService userService;

        public BrowserHub(
            IForwardingClient<PayloadType, string> forwardingClient,
            IConnectionIdGenerator connectionIdGenerator,
            IUserService userService)
        {
            this.forwardingClient = forwardingClient;
            this.connectionIdGenerator = connectionIdGenerator;
            this.forwardingClient.Start();
            this.userService = userService;

            this.SetupForwarding();
        }

        private void SetupForwarding()
        {
            this.forwardingClient.Receive.Subscribe(env =>
            {
                if (this.connections.GetConnection(env.ConnectionId, out var node))
                {
                    Clients.Client(node.ExternalConnection).Received(
                        env);
                }
            });
        }

        /// <summary>
        ///     Used to forward connections to the backend.
        /// </summary>
        public void Send(
            MessageEnvelope<PayloadType, string> envelope)
        {
            this.forwardingClient.Send(
                envelope);
        }

        /// <summary>
        ///     Used to authenticate a user.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>The task that performs the lookup.</returns>
        public async Task Auth(
            string username,
            string password)
        {
            var user = await this.userService.Login(
                new UserLoginRq
                {
                    Username = username,
                    Password = password
                });

            if (user.Success)
            {
                var connectionId = this.connectionIdGenerator.Generate();
                this.connections.AddOrUpdate(
                    connectionId,
                    Context.ConnectionId);
                Clients.Caller.AuthResponse(true, connectionId);
            }
            else
            {
                Clients.Caller.AuthResponse(false, Guid.Empty);
            }
        }

        /// <summary>
        ///     Called to ping the server.
        /// </summary>
        /// <param name="connectionId">The id of the connection.</param>
        public void Ping(Guid connectionId)
        {
            this.connections.AddOrUpdate(connectionId, Context.ConnectionId);
            Clients.Caller.Pong();
        }

        public interface IBrowserClient
        {
            /// <summary>
            ///     The receive function on the client.
            /// </summary>
            void Received(
                MessageEnvelope<PayloadType, string> envelope);

            void AuthResponse(
                bool success,
                Guid connectionId);

            void Pong();
        }
    }
}