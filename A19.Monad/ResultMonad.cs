using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using A19.Core;

namespace Mrh.Monad
{

    public interface IResultMonad<T>
    {
        
    }

    public class ResultSuccess<T> : IResultMonad<T>
    {

        public readonly T Result;
        
        public ResultSuccess(T value)
        {
            this.Result = value;
        }
    }

    public interface IResultError<T> : IResultMonad<T>
    {

        IResultMonad<T2> To<T1, T2>();
    }

    public class ResultError<T> : IResultError<T>
    {

        public readonly List<string> errors;
        
        /// <summary>
        ///     Used to create an error monad.
        /// </summary>
        /// <param name="errors">A human readable list of errors.</param>
        public ResultError(List<string> errors)
        {
            this.errors = errors;
        }

        public IResultMonad<T2> To<T1, T2>()
        {
            return new ResultError<T2>(this.errors);
        }
    }
    
    /// <summary>
    ///     The binds for the result monad.
    /// </summary>
    public static class ResultMonad
    {
        /// <summary>
        ///     Bind when the parameters are synchronous.
        /// </summary>
        /// <param name="monad">The monad.</param>
        /// <param name="func">The function</param>
        /// <typeparam name="T1">The initial type.</typeparam>
        /// <typeparam name="T2">The to type.</typeparam>
        /// <returns>The result.</returns>
        /// <exception cref="Exception"> When the types to don't match.</exception>
        public static IResultMonad<T2> Bind<T1, T2>(
            this IResultMonad<T1> monad,
            Func<T1, IResultMonad<T2>> func)
        {
            if (monad is ResultSuccess<T1> success)
            {
                return func(success.Result);
            }

            if (monad is ResultError<T1> error)
            {
                return error.To<T1, T2>();
            }
            throw new UnknownTypeException(monad.GetType(), $"Unable to determine how to bind the monad {monad.GetType()}.");
        }

        public static Task<IResultMonad<T2>> Bind<T1, T2>(
            this IResultMonad<T1> monad,
            Func<T1, Task<IResultMonad<T2>>> func)
        {
            if (monad is ResultSuccess<T1> success)
            {
                return func(success.Result);
            }

            if (monad is ResultError<T1> error)
            {
                return Task.FromResult(error.To<T1, T2>());
            }
            throw new UnknownTypeException(monad.GetType(), $"Unable to determine how to bind the monad {monad.GetType()}.");
        }
        public static async Task<IResultMonad<T2>> Bind<T1, T2>(
            this Task<IResultMonad<T1>> monadT,
            Func<T1, Task<IResultMonad<T2>>> func)
        {
            var monad = await monadT.ConfigureAwait(false);
            if (monad is ResultSuccess<T1> success)
            {
                return await func(success.Result).ConfigureAwait(false);
            }

            if (monad is ResultError<T1> error)
            {
                return error.To<T1, T2>();
            }
            throw new UnknownTypeException(monad.GetType(), $"Unable to determine how to bind the monad {monad.GetType()}.");
        }

        public static async Task<IResultMonad<T2>> Bind<T1, T2>(
            this Task<IResultMonad<T1>> monadT,
            Func<T1, IResultMonad<T2>> func)
        {
            var monad = await monadT.ConfigureAwait(false);
            if (monad is ResultSuccess<T1> success)
            {
                return func(success.Result);
            }

            if (monad is ResultError<T1> error)
            {
                return error.To<T1, T2>();
            }
            throw new UnknownTypeException(monad.GetType(), $"Unable to determine how to bind the monad {monad.GetType()}.");
        }

        /// <summary>
        ///     Converts the ValidationError to a result monad.
        /// </summary>
        /// <param name="errors">The list of errors.</param>
        /// <typeparam name="T">The type of the monad.</typeparam>
        /// <returns>The resulting monad.</returns>
        public static IResultMonad<T> Lift<T>(
            this List<ValidationError> errors)
        {
            if (errors.Any())
            {
                return new ResultError<T>(errors.Select(v => v.Description).ToList());
            }
            else
            {
                return new ResultSuccess<T>(default);
            }
        }

        public static IResultMonad<T> ToResultMonad<T>(
            this T value)
        {
            return new ResultSuccess<T>(value);
        }
    }
}