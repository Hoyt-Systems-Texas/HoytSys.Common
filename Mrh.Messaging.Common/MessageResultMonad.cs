using System.Collections.Generic;
using System.Threading.Tasks;
using Mrh.Monad;

namespace Mrh.Messaging.Common
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
        public Task<IResultMonad<T>> To<T>(Message<TPayloadType, TBody> message)
        {
            switch (message.MessageResultType)
            {
                case MessageResultType.Busy:
                    return Task.FromResult((IResultMonad<T>) new ResultMonadBusy<T>());
                
                case MessageResultType.Error:
                    return Task.FromResult((IResultMonad<T>) new ResultError<T>(this.bodyEncoder.Decode<List<string>>(message.Body)));
                    
                case MessageResultType.Success:
                    return Task.FromResult(
                        (IResultMonad<T>) new ResultSuccess<T>(this.bodyEncoder.Decode<T>(message.Body)));
                
                case MessageResultType.AccessDenied:
                    return Task.FromResult(
                        (IResultMonad<T>) new ResultAccessDenied<T>(
                            this.bodyEncoder.Decode<List<string>>(message.Body)));
                
                default:
                    return Task.FromResult((IResultMonad<T>) new ResultError<T>(new List<string>
                    {
                        $"Unknown result type of {message.MessageResultType}"
                    }));
            }
        }
    }
}