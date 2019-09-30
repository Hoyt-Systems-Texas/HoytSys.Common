using System;
using System.Threading.Tasks;
using A19.Messaging.Client;
using A19.Messaging.Common;
using A19.Security.User;
using A19.User.Common;
using Microsoft.AspNetCore.SignalR;
using Mrh.Messaging;
using NLog;
using ServiceApplicationTester;

namespace TestApp.Hubs
{
    public class BrowserHub : Hub<BrowserHub.IBrowserClient>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly IForwardingClient<PayloadType, string> forwardingClient;
        private readonly IUserService userService;
        private readonly ConnectionManager connectionManager;

        public BrowserHub(
            IForwardingClient<PayloadType, string> forwardingClient,
            ConnectionManager connectionManager,
            IUserService userService)
        {
            this.connectionManager = connectionManager;
            this.forwardingClient = forwardingClient;
            this.userService = userService;
        }

        /// <summary>
        ///     Used to forward connections to the backend.
        /// </summary>
        public void SendEnv(
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
            try
            {
                var user = await this.userService.Login(
                    new UserLoginRq
                    {
                        Username = username,
                        Password = password
                    });

                if (user.Success)
                {
                    var connectionId = this.connectionManager.RegisterConnection(Context.ConnectionId, user.UserGuid);
                    await Clients.Caller.AuthResponse(true, connectionId);
                }
                else
                {
                    await Clients.Caller.AuthResponse(false, Guid.Empty);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, ex.Message);
            }
        }

        /// <summary>
        ///     Called to ping the server.
        /// </summary>
        /// <param name="connectionId">The id of the connection.</param>
        public void Ping(Guid connectionId)
        {
            this.connectionManager.Update(connectionId);
            Clients.Caller.Pong();
        }

        public interface IBrowserClient
        {
            /// <summary>
            ///     The receive function on the client.
            /// </summary>
            Task Received(
                MessageEnvelope<PayloadType, string> envelope);

            Task AuthResponse(
                bool success,
                Guid connectionId);

            Task Pong();
        }
    }
}