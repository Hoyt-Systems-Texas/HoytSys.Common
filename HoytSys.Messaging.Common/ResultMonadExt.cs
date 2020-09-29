using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mrh.Monad;

namespace A19.Messaging.Common
{
    public static class ResultMonadExt
    {
        public static async Task<IResultMonad<T>> Error<T>(
            this Task<IResultMonad<T>> monadT,
            Func<List<string>, Task> handler)
        {
            var monad = await monadT;
            if (monad is ResultError<T> error)
            {
                await handler(error.errors);
            }

            return monad;
        }

        public static async Task<IResultMonad<T>> Busy<T>(
            this Task<IResultMonad<T>> monadT,
            Func<Task> handler)
        {
            var monad = await monadT;
            if (monad is ResultMonadBusy<T> busy)
            {
                await handler();
            }

            return monad;
        }

        public static async Task<IResultMonad<T>> AccessDenied<T>(
            this Task<IResultMonad<T>> monadT,
            Func<List<string>, Task> handler)
        {
            var monad = await monadT;
            if (monad is ResultAccessDenied<T> accessDenied)
            {
                await handler(accessDenied.Errors);
            }

            return monad;
        }

        public static async Task<IResultMonad<T>> AllErrors<T>(
            this Task<IResultMonad<T>> monadT,
            Func<Task> handler)
        {
            var monad = await monadT;
            if (monad is IResultError<T> error)
            {
                await handler();
            }

            return monad;
        }
    }

    public class ResultMonadBusy<T> : IResultError<T>
    {
        public IResultMonad<T2> To<T1, T2>()
        {
            return new ResultMonadBusy<T2>();
        }
    }

    public class ResultAccessDenied<T> : IResultError<T>
    {

        public readonly List<string> Errors;

        public ResultAccessDenied(
            List<string> errors)
        {
            this.Errors = errors;
        }
        
        public IResultMonad<T2> To<T1, T2>()
        {
            return new ResultAccessDenied<T2>(this.Errors);
        }
    }
    
}