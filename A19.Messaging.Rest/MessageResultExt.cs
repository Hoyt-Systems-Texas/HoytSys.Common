using System;
using Mrh.Monad;

namespace A19.Messaging.Rest
{
    public static class MessageResultExt
    {
        public static IMessageResult ToMessageResult<T>(this IResultMonad<T> result)
        {
            switch (result)
            {
                case ResultSuccess<T> s:
                    return new MessageResultSuccess<T>
                    {
                        Result = s.Result
                    };
                case ResultError<T> e:
                    return new MessageResultError
                    {
                        Errors = e.errors
                    };
                
                default:
                    throw new Exception($"Unable to convert to a message type {result}");
            }
        }
    }
}