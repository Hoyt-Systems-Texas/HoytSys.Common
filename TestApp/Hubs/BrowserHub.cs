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
                        env.RequestId,
                        env.ConnectionId,
                        env.CorrelationId,
                        env.Number,
                        env.Total,
                        env.TotalBodyLength,
                        env.MessageType,
                        env.PayloadType,
                        env.MessageResultType,
                        env.ToConnectionId,
                        env.Body);
                }
            });
        }

        /// <summary>
        ///     Used to forward connections to the backend.
        /// </summary>
        /// <param name="requestId">The request id.</param>
        /// <param name="connectionId">The id of the connection.</param>
        /// <param name="correlationId"></param>
        /// <param name="number">The fragment number.</param>
        /// <param name="total">The total number of fragments.</param>
        /// <param name="totalBodyLength">The total body length.</param>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="payloadType">The payload type.</param>
        /// <param name="messageResultType">The message result type.</param>
        /// <param name="toConnectionId">Who to send the message to.</param>
        /// <param name="body">The body of the message.</param>
        public void Send(
            long requestId,
            Guid connectionId,
            int correlationId,
            int number,
            int total,
            int totalBodyLength,
            MessageType messageType,
            PayloadType payloadType,
            MessageResultType messageResultType,
            Guid toConnectionId,
            string body)
        {
            this.forwardingClient.Send(
                new MessageEnvelope<PayloadType, string>
                {
                    RequestId = requestId,
                    ConnectionId = connectionId,
                    CorrelationId = correlationId,
                    Number = number,
                    Total = total,
                    TotalBodyLength = totalBodyLength,
                    MessageType = messageType,
                    PayloadType = payloadType,
                    MessageResultType = messageResultType,
                    ToConnectionId = toConnectionId,
                    Body = body
                });
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
            /// <param name="requestId"></param>
            /// <param name="connectionId"></param>
            /// <param name="correlationId"></param>
            /// <param name="number"></param>
            /// <param name="total"></param>
            /// <param name="totalBodyLength"></param>
            /// <param name="messageType"></param>
            /// <param name="payloadType"></param>
            /// <param name="messageResultType"></param>
            /// <param name="toConnectionId"></param>
            /// <param name="body"></param>
            void Received(
                long requestId,
                Guid connectionId,
                int correlationId,
                int number,
                int total,
                int totalBodyLength,
                MessageType messageType,
                PayloadType payloadType,
                MessageResultType messageResultType,
                Guid toConnectionId,
                string body);

            void AuthResponse(
                bool success,
                Guid connectionId);

            void Pong();
        }
    }
}