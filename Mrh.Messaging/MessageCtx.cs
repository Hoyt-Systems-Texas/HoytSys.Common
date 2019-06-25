using System;
using System.Threading;

namespace Mrh.Messaging
{
    public class MessageCtx<TPayloadType, TBody> : IMessageCtx<TPayloadType, TBody>
    {

        private int currentState = 0;
        
        public long RequestId { get; set; }
        
        public Guid UserId { get; set; }
        
        public MessageIdentifier MessageIdentifier { get; set; }
        
        public MessageType MessageType { get; set; }
        
        public TPayloadType PayloadType { get; set; }

        public TBody Body { get; set; }

        public MessageState CurrentState
        {
            get { return (MessageState) this.currentState; }
        }

        public bool TrySetState(MessageState newState, MessageState oldState)
        {
            var newStateInt = (int) newState;
            var oldStateInt = (int) oldState;
            return Interlocked.CompareExchange(
                       ref this.currentState,
                       newStateInt,
                       oldStateInt) == oldStateInt;
        }
    }
}