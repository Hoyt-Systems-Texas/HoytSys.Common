using System.Threading.Tasks;
using Mrh.Monad;

namespace A19.User.Common
{
    public interface IUserClient
    {
        /// <summary>
        ///     Used to login a user into a system.
        /// </summary>
        /// <param name="request">The request for logging in the user.</param>
        /// <returns>The result monad.</returns>
        Task<IResultMonad<UserLoginRs>> Login(UserLoginRq request);

        Task<IResultMonad<UserSessionExtRs>> ExtendSession(UserSessionExtRq request);
        
        Task<IResultMonad<DestroySessionRs>> DestroySession(DestroySessionRq request);
    }
}