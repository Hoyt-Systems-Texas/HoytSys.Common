using System.Collections.Generic;
using Mrh.Monad;

namespace A19.Messaging.Common
{
    
    public class MessageResultMonad<TPayloadType, TBody> : IMessageResultMonad<TPayloadType, TBody> where TPayloadType: struct
    {
        
        private readonly IBodyEncoder<TBody> bodyEncoder;

        public MessageResultMonad(
            IBodyEncoder<TBody> bodyEncoder)
        {
            this.bodyEncoder = bodyEncoder;
        }
        
        
        /// <summary>
        ///     Used to convert a message to a message result type.
        /// </summary>
        /// <param name="message">The message to convert.</param>
        /// <typeparam name="T">The type of the body.</typeparam>
        /// <returns>The result monad.</returns>
        public IResultMonad<T> To<T>(Message<TPayloadType, TBody> message)
        {
            switch (message.MessageResultType)
            {
                case MessageResultType.Busy:
                    return new ResultMonadBusy<T>();
                
                case MessageResultType.Error:
                    return new ResultError<T>(this.bodyEncoder.Decode<List<string>>(message.Body));
                    
                case MessageResultType.Success:
                    return new ResultSuccess<T>(this.bodyEncoder.Decode<T>(message.Body));
                
                case MessageResultType.AccessDenied:
                    return new ResultAccessDenied<T>(
                            this.bodyEncoder.Decode<List<string>>(message.Body));
                
                default:
                    return new ResultError<T>(new List<string>
                    {
                        $"Unknown result type of {message.MessageResultType}"
                    });
            }
        }
    }
}