using System;
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

        /// <summary>
        ///    Used to get a user information.
        /// </summary>
        /// <param name="userId">The id of the user to get the information for.</param>
        /// <returns>The result of trying to get the user.</returns>
        Task<IResultMonad<Common.User>> Get(Guid userId);
    }
}