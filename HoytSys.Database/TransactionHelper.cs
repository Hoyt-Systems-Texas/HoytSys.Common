using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace A19.Database
{
    public static class TransactionHelper
    {
        public static Task<T> Uncommitted<T>(this SqlConnection conn, Func<SqlConnection, SqlTransaction, Task<T>> func)
        {
            return conn.SqlFetch(IsolationLevel.ReadUncommitted, func);
        }

        public static Task<T> Committed<T>(this SqlConnection conn, Func<SqlConnection, SqlTransaction, Task<T>> func)
        {
            return conn.SqlFetch(IsolationLevel.ReadCommitted, func);
        }

        public static Task Committed(this SqlConnection conn, Func<SqlConnection, SqlTransaction, Task> func)
        {
            return conn.SqlUpdate(IsolationLevel.ReadCommitted, func);
        }

        public static async Task<T> SqlFetch<T>(this SqlConnection conn, IsolationLevel level, Func<SqlConnection, SqlTransaction, Task<T>> func)
        {
            using (var trans = conn.BeginTransaction(level))
            {
                try {
                    var result = await func(conn, trans);
                    trans.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public static async Task SqlUpdate(this SqlConnection conn, IsolationLevel isolationLevel,
            Func<SqlConnection, SqlTransaction, Task> func)
        {
            using (var trans = conn.BeginTransaction(isolationLevel))
            {
                try
                {
                    await func(conn, trans);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }
}