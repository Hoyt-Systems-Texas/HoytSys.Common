using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace A19.Database
{
    public interface IDatabaseTransaction<TConn, TTrans> where TConn:DbConnection
        where TTrans:DbTransaction
    {
        /// <summary>
        ///     Used to create a read uncommitted transaction.
        /// </summary>
        /// <param name="func">The function to run.</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The task that performs the query.</returns>
        Task<T> Uncommitted<T>(Func<TConn, TTrans, Task<T>> func);

        /// <summary>
        ///     Used to create a read committed transaction.
        /// </summary>
        /// <param name="func">The function to execute in the transaction.</param>
        /// <typeparam name="T">The return type.</typeparam>
        /// <returns>The task that runs the update.</returns>
        Task<T> Committed<T>(Func<TConn, TTrans, Task<T>> func);

        /// <summary>
        ///     Used to create a read committed transaction that doesn't have a return type.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        /// <returns>The task that performs the update.</returns>
        Task Committed(Func<TConn, TTrans, Task> func);
    }
}