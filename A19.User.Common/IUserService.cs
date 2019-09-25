using System.Threading.Tasks;
using A19.User.Common;

namespace A19.Security.User
{
    public interface IUserService
    {
        /// <summary>
        ///     Used to login a user.
        /// </summary>
        /// <param name="userLoginRq">The user login information.</param>
        /// <returns>The user login response.</returns>
        Task<UserLoginRs> Login(UserLoginRq userLoginRq);
    }
}