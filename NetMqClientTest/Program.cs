using System;
using A19.Messaging;
using A19.Messaging.Client;
using A19.Messaging.Common;
using A19.Messaging.NetMq;
using A19.Messaging.NetMq.Client;
using Mrh.Messaging.Json;
using NetMqTestCommon;
using NLog;
using ServiceApplicationTester;
using Unity;

namespace NetMqClientTest
{
    class Program
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static UnityContainer unityContainer = new UnityContainer();
        private static NetMqForwardingClientRqRs<PayloadType, string> forwardingClientRqRs;

        static void Main(string[] args)
        {
            try
            {
                SetupUnity();
                var result = forwardingClientRqRs.Send(
                    PayloadType.SendTest,
                    "Hi",
                    Guid.NewGuid());
                result.Wait();
                Console.WriteLine($"The result was {result.Result.MessageResultType}");
            }
            catch (Exception ex)
            {
                log.Error(ex, ex.Message);
            }
        }

        private static void SetupUnity()
        {
            unityContainer.RegisterSingleton<
                    IBodyReconstructorFactory<string>,
                    JsonBodyReconstructorFactory>()
                .RegisterSingleton<
                    IEnvelopFactory<PayloadType, string>,
                    JsonEnvelopeFactory<PayloadType>>()
                .RegisterSingleton<
                    IMessageResultFactory<string>,
                    JsonMessageResultFactory>()
                .RegisterSingleton<
                    IEncoder<PayloadType, string>,
                    StringBodyEncoder<PayloadType>>()
                .RegisterSingleton<
                    IJsonSetting,
                    JsonSettings>()
                .RegisterSingleton<
                    IOutgoingConnection<PayloadType, string>,
                    OutgoingConnection<PayloadType, string>>()
                .RegisterSingleton<
                    IConnectionIdGenerator,
                    ConnectionIdGenerator>()
                .RegisterSingleton<
                    IIncomingMessageBuilder<PayloadType, string>,
                    IncomingMessageBuilder<PayloadType, string>>()
                .RegisterSingleton<
                    NetMqForwardingClientRqRs<PayloadType, string>>()
                .RegisterSingleton<
                    IncomingConnection<PayloadType, string, MessageCtx<PayloadType, string>>>()
                .RegisterSingleton<
                    IEncoder<PayloadType, string>,
                    StringBodyEncoder<PayloadType>>()
                .RegisterSingleton<
                    INetMqConfig,
                    NetMqConfig>()
                .RegisterSingleton<
                    IBodyEncoder<string>,
                    JsonBodyEncoder>()
                .RegisterSingleton<
                    IMessageSetting,
                    MessagingSetting>()
                .RegisterSingleton<
                    IPayloadTypeEncoder<PayloadType, string>,
                    PayloadTypeEncoder>()
                ;
            forwardingClientRqRs = unityContainer.Resolve<NetMqForwardingClientRqRs<PayloadType, string>>();
            unityContainer.RegisterInstance<
                    IIncomingMessageHandler<PayloadType, string>>(forwardingClientRqRs)
                .RegisterInstance<IForwardingClientRqRs<PayloadType, string>>(forwardingClientRqRs);


            var outgoingConnection = unityContainer.Resolve<IOutgoingConnection<PayloadType, string>>();
            var incomingConnection = unityContainer
                .Resolve<IncomingConnection<PayloadType, string, MessageCtx<PayloadType, string>>>();
            outgoingConnection.Connect();
            incomingConnection.Start();
        }
    }
}