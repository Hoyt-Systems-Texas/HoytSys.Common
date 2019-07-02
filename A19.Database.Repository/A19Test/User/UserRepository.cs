using System.Threading.Tasks;
using Dapper;

namespace A19.Database.Repository.A19Test.User
{
    public class UserRepository : IUserRepository
    {

        private readonly A19DatabaseConnection connection;

        public UserRepository(
            A19DatabaseConnection connection)
        {
            this.connection = connection;
        }

        public Task<NetMqTestCommon.User> Get(string username)
        {
            return this.connection.Uncommitted(async (conn, trans) =>
            {
                var sql = @"usp_GetUserByUsername";
                return await conn.QueryFirstOrDefaultAsync<NetMqTestCommon.User>(
                    sql,
                    new
                    {
                        username
                    },
                    trans);
            });
        }
    }

}