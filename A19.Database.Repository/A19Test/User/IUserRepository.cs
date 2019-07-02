using System.Threading.Tasks;

namespace A19.Database.Repository.A19Test.User
{
    public interface IUserRepository
    {

        /// <summary>
        ///     Used to get a user record by the username.
        /// </summary>
        /// <param name="username">The username of the user to get the record for.</param>
        /// <returns>Null or the user with the specified name.</returns>
        Task<NetMqTestCommon.User> Get(string username);
    }
}