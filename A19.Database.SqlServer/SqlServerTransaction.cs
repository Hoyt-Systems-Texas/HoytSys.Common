using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace A19.Database.SqlServer
{
    public class SqlServerTransaction : IDatabaseTransaction<SqlConnection, SqlTransaction>
    {
        private readonly string connection;

        public SqlServerTransaction(string connection)
        {
            this.connection = connection;
        }
        
        public Task<T> Uncommitted<T>(Func<SqlConnection, SqlTransaction, Task<T>> func)
        {
            return ResultTrans(IsolationLevel.ReadUncommitted, func);
        }

        public Task<T> Committed<T>(Func<SqlConnection, SqlTransaction, Task<T>> func)
        {
            return ResultTrans(IsolationLevel.ReadCommitted, func);
        }

        public Task Committed(Func<SqlConnection, SqlTransaction, Task> func)
        {
            return Trans(IsolationLevel.ReadCommitted, func);
        }

        private async Task<T> ResultTrans<T>(
            IsolationLevel isolationLevel,
            Func<SqlConnection, SqlTransaction, Task<T>> func)
        {
            using (var conn = new SqlConnection(this.connection))
            {
                await conn.OpenAsync();
                using (var trans = conn.BeginTransaction(isolationLevel))
                {
                    try
                    {
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
        }

        private async Task Trans(
            IsolationLevel isolationLevel,
            Func<SqlConnection, SqlTransaction, Task> func)
        {
            using (var conn = new SqlConnection(this.connection))
            {
                await conn.OpenAsync();
                using (var trans = conn.BeginTransaction(isolationLevel))
                {
                    try
                    {
                        await func(conn, trans);
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
}