using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace A19.Database.Repository.A19Test
{
    public class A19DatabaseConnection : IDatabaseTransaction<SqlConnection, SqlTransaction>
    {

        private readonly string databaseConnection;

        public A19DatabaseConnection(
            IConfiguration configuration)
        {
            this.databaseConnection = configuration.GetConnectionString("TestDb");
        }
        
        public async Task<T> Uncommitted<T>(Func<SqlConnection, SqlTransaction, Task<T>> func)
        {
            using (var sqlConn = new SqlConnection(this.databaseConnection))
            {
                await sqlConn.OpenAsync();
                return await sqlConn.Uncommitted(func);
            }
        }

        public async Task<T> Committed<T>(Func<SqlConnection, SqlTransaction, Task<T>> func)
        {
            using (var sqlConn = new SqlConnection(this.databaseConnection))
            {
                await sqlConn.OpenAsync();
                return await sqlConn.Committed(func);
            }
        }

        public async Task Committed(Func<SqlConnection, SqlTransaction, Task> func)
        {
            using (var sqlConn = new SqlConnection(this.databaseConnection))
            {
                await sqlConn.OpenAsync();
                await sqlConn.Committed(func);
            }
        }
    }
}