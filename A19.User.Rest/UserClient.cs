using System.Threading.Tasks;
using A19.Messaging.Rest;
using A19.User.Common;
using Mrh.Monad;

namespace A19.User.Rest
{
    public class UserClient : IUserClient
    {

        private readonly IRestClient _restClient;

        public UserClient(
            IUserClientSettings userClientSettings)
        {
            _restClient = new RestSystemClient(userClientSettings.UserLoginUrl);
        }

        /// <summary>
        ///     Used to login a user into a system.
        /// </summary>
        /// <param name="request">The request for logging in the user.</param>
        /// <returns>The result monad.</returns>
        public async Task<IResultMonad<UserLoginRs>> Login(UserLoginRq request)
        {
            return await _restClient.PostAsync<UserLoginRq, UserLoginRs>(
                "Login",
                "login", request);
        }

        public async Task<IResultMonad<UserSessionExtRs>> ExtendSession(UserSessionExtRq request)
        {
            return await _restClient.PostAsync<UserSessionExtRq, UserSessionExtRs>(
                "Login",
                "extend-session",
                request);
        }

        public async Task<IResultMonad<DestroySessionRs>> DestroySession(DestroySessionRq request)
        {
            return await _restClient.PostAsync<DestroySessionRq, DestroySessionRs>(
                "Login",
                "destroy-session",
                request);
        }
    }
}