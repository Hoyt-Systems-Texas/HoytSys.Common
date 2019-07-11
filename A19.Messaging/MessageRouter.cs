using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A19.Messaging
{
    public class MessageRouter<TPayloadType, TBody, TCtx> : IMessageRouter<TPayloadType, TBody, TCtx> where TPayloadType:struct where TCtx:IMessageCtx<TPayloadType, TBody>
    {

        private readonly Dictionary<TPayloadType, Node> handlers = new Dictionary<TPayloadType, Node>(1000);
        private readonly IMessageResultFactory<TBody> messageResultFactory;
        private readonly List<Func<TCtx, Task>> preprocessors = new List<Func<TCtx, Task>>(10);

        public MessageRouter(IMessageResultFactory<TBody> messageResultFactory)
        {
            this.messageResultFactory = messageResultFactory;
        }

        public IMessageRouter<TPayloadType, TBody, TCtx> AddPreprocessor(Func<TCtx, Task> preprocessor)
        {
            this.preprocessors.Add(preprocessor);
            return this;
        }

        private async Task RunPreprocessors(TCtx msgCtx)
        {
            foreach (var preprocessor in this.preprocessors)
            {
                await preprocessor(msgCtx);
            }
        }

        public async Task<MessageResult<TBody>> Route(TCtx msgCtx) 
        {
            if (this.handlers.TryGetValue(msgCtx.PayloadType, out Node node))
            {
                if (await node.Access(msgCtx))
                {
                    await this.RunPreprocessors(msgCtx);
                    return await node.Handler(msgCtx);
                }
                else
                {
                    return this.messageResultFactory.AccessDenied();
                }
            }
            else
            {
                throw new UnknownHandlerException<TPayloadType>(msgCtx.PayloadType);
            }
        }
        
        public IMessageRouter<TPayloadType, TBody, TCtx> Register(TPayloadType payloadType, Func<TCtx, Task<bool>> accessFunc, Func<TCtx, Task<MessageResult<TBody>>> handler)
        {
            if (this.handlers.ContainsKey(payloadType))
            {
                throw new AlreadyRegisteredException<TPayloadType>(payloadType);
            }
            this.handlers[payloadType] = new Node(
                payloadType,
                accessFunc,
                handler);
            return this;
        }

        private struct Node
        {
            public readonly TPayloadType PayloadType;
            public readonly Func<TCtx, Task<bool>> Access;
            public readonly Func<TCtx, Task<MessageResult<TBody>>> Handler;

            public Node(
                TPayloadType payloadType,
                Func<TCtx, Task<bool>> access,
                Func<TCtx, Task<MessageResult<TBody>>> handler)
            {
                this.PayloadType = payloadType;
                this.Access = access;
                this.Handler = handler;
            }
        }

    }
}