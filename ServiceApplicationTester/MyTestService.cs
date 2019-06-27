using System;
using System.Threading;
using Mrh.Concurrent;
using Mrh.Core;
using Mrh.Messaging;
using Mrh.Messaging.Json;
using Mrh.Messaging.NetMq;
using NLog;
using Topshelf;
using Unity;

namespace ServiceApplicationTester
{
    public class MyTestService : IStartable, IStoppable
    {

        private readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly UnityContainer container;
        private IOutgoingConnection<PayloadType, string> outgoingConnection;
        private IncomingConnection<PayloadType, string, MessageCtx<PayloadType, string>> incomingConnection;
        private IIncomingMessageHandler<PayloadType, string> incomingMessageHandler;
        

        public MyTestService()
        {
            this.container = new UnityContainer();
            this.SetupUnity();
        }

        private void SetupUnity()
        {
            try
            {
                this.container
                    .RegisterSingleton<
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
                        IMessageRouter<PayloadType, string, MessageCtx<PayloadType, string>>,
                        MessageRouter<PayloadType, string, MessageCtx<PayloadType, string>>>()
                    .RegisterSingleton<
                        IRequestIdGenerator,
                        RequestIdGenerator>()
                    .RegisterSingleton<
                        IMessageStore<PayloadType, string, MessageCtx<PayloadType, string>>,
                        InMemoryMessageStore<PayloadType, string, MessageCtx<PayloadType, string>>>()
                    .RegisterSingleton<
                        IIncomingMessageHandler<PayloadType, string>,
                        IncomingMessageProcessor<PayloadType, string, MessageCtx<PayloadType, string>>>()
                    .RegisterSingleton<IncomingConnection<PayloadType, string, MessageCtx<PayloadType, string>>>()
                    .RegisterSingleton<
                        IEncoder<PayloadType, string>,
                        StringBodyEncoder<PayloadType>>()
                    .RegisterSingleton<
                        INetMqConfig,
                        NetMqConfig>()
                    .RegisterSingleton<
                        IPayloadTypeEncoder<PayloadType, string>,
                        PayloadTypeEncoder>()
                    .RegisterSingleton<
                        IMessageSetting,
                        MessagingSetting>()
                    ;

                this.outgoingConnection = this.container.Resolve<IOutgoingConnection<PayloadType, string>>();
                this.incomingConnection = this.container
                    .Resolve<IncomingConnection<PayloadType, string, MessageCtx<PayloadType, string>>>();
                this.incomingMessageHandler = this.container
                    .Resolve<IIncomingMessageHandler<PayloadType, string>>();
            }
            catch (Exception ex)
            {
                log.Error(ex, ex.Message);
                throw;
            }

        }
        
        public void Start()
        {
            log.Info("Starting..");
            try
            {
                this.outgoingConnection.Connect();
                this.incomingConnection.Start();
                this.incomingMessageHandler.Start();
                log.Info("Started");
            }
            catch (Exception ex)
            {
                log.Error(ex, ex.Message);
            }
        }

        public void Stop()
        {
            log.Info("Stopping...");
            try
            {
                this.outgoingConnection.Dispose();
                this.incomingConnection.Stop();
                this.incomingMessageHandler.Stop();
                log.Info("Stopped");
            }
            catch (Exception ex)
            {
                log.Error(ex, ex.Message);
            }
        }
    }
}