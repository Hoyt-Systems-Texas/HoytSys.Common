using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Mrh.Database;

namespace A19.Database.Repository.A19Test
{
    public class A19DatabaseConnection : IDatabaseTransaction
    {

        private readonly string databaseConnection;

        public A19DatabaseConnection(
            IConfiguration configuration)
        {
            this.databaseConnection = configuration.GetConnectionString("TestDb");
        }
        
        public Task<T> Uncommitted<T>(Func<SqlConnection, SqlTransaction, Task<T>> func)
        {
            using (var sqlConn = new SqlConnection(this.databaseConnection))
            {
                return sqlConn.Uncommitted(func);
            }
        }

        public Task<T> Committed<T>(Func<SqlConnection, SqlTransaction, Task<T>> func)
        {
            using (var sqlConn = new SqlConnection(this.databaseConnection))
            {
                return sqlConn.Committed(func);
            }
        }

        public Task Committed(Func<SqlConnection, SqlTransaction, Task> func)
        {
            using (var sqlConn = new SqlConnection(this.databaseConnection))
            {
                return sqlConn.Committed(func);
            }
        }
    }
}