using System.Collections.Concurrent;
using System.Threading.Tasks;
using A19.Messaging.Common;

namespace A19.Messaging
{
    public class InMemoryMessageStore<TPayloadType, TBody, TCtx> : IMessageStore<TPayloadType, TBody, TCtx>
        where TPayloadType : struct
        where TCtx: IMessageCtx<TPayloadType, TBody>
    {

        private readonly IRequestIdGenerator requestIdGenerator;
        private readonly ConcurrentDictionary<MessageIdentifier, TCtx> requestStore = new ConcurrentDictionary<MessageIdentifier, TCtx>(10, 1000);
        private readonly ConcurrentDictionary<MessageIdentifier, MessageResult<TBody>> results = new ConcurrentDictionary<MessageIdentifier, MessageResult<TBody>>(10, 1000);

        public InMemoryMessageStore(
            IRequestIdGenerator requestIdGenerator)
        {
            this.requestIdGenerator = requestIdGenerator;
        }

        public Task<TCtx> Save(TCtx msgCtx)
        {
            if (!this.requestStore.TryAdd(msgCtx.MessageIdentifier, msgCtx))
            {
                this.requestStore.TryGetValue(msgCtx.MessageIdentifier, out msgCtx);
            }
            else
            {
                msgCtx.RequestId = this.requestIdGenerator.Next();
            }
            return Task.FromResult(msgCtx);
        }

        public Task<(bool, TCtx)> TryGet(MessageIdentifier identifier)
        {
            TCtx context;
            if (this.requestStore.TryGetValue(identifier, out context))
            {
                return Task.FromResult((true, context));
            }
            return Task.FromResult<(bool, TCtx)>((false, default));
        }

        public Task Save(MessageResult<TBody> result)
        {
            this.results.TryAdd(result.MessageIdentifier, result);
            return Task.FromResult(0);
        }

        public Task<(bool, MessageResult<TBody>)> TryGetResult(MessageIdentifier identifier)
        {
            MessageResult<TBody> result;
            if (this.results.TryGetValue(identifier, out result))
            {
                return Task.FromResult<(bool, MessageResult<TBody>)>((true, result));
            }

            return Task.FromResult((false, (MessageResult<TBody>) null));
        }
    }
}