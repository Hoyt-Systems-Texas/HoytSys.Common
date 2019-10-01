using System;
using System.Collections.Generic;
using A19.Messaging.Common;
using Mrh.Messaging.Json;
using Mrh.Monad;

namespace A19.Messaging.Rest
{
    public static class MessageResultExt
    {
        public static IMessageResult<T> ToMessageResult<T>(this IResultMonad<T> result)
        {
            switch (result)
            {
                case ResultSuccess<T> s:
                    return new MessageResultSuccess<T>
                    {
                        Result = s.Result
                    };
                case ResultError<T> e:
                    return new MessageResultError<T>
                    {
                        Errors = e.errors
                    };
                
                default:
                    throw new Exception($"Unable to convert to a message type {result}");
            }
        }

        /// <summary>
        ///     Used to convert a message result type json string to IResultMonad.
        /// </summary>
        /// <param name="resultType">The result type.</param>
        /// <param name="body">The body of the message.</param>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <returns>The result monad.</returns>
        public static IResultMonad<T> ToResultMonad<T>(this MessageResultType resultType, string body)
        {
            switch (resultType)
            {
                case MessageResultType.Success:
                    var message = JsonHelper.Decode<MessageResultSuccess<T>>(body);
                    return new ResultSuccess<T>(message.Result);
                
                case MessageResultType.AccessDenied:
                    return new ResultAccessDenied<T>(new List<string>
                    {
                        "Access denied"
                    });
                
                default:
                    var messageE = JsonHelper.Decode<MessageResultError<T>>(body);
                    return new ResultError<T>(messageE.Errors);
            }
        }
    }
}