using System.Threading.Tasks;
using A19.Messaging.Rest;
using A19.User.Common;
using Mrh.Monad;

namespace A19.System.Rest
{
    /// <summary>
    ///     The rest client for connecting to a system.
    /// </summary>
    public class SystemClient : ISystemClient
    {

        private readonly IRestClient _restClient;

        public SystemClient(
            IUserClientSettings userClientSettings)
        {
            _restClient = new RestSystemClient(userClientSettings.UserLoginUrl);
        }

        public async Task<IResultMonad<SystemLoginRs>> Login(SystemLoginRq systemLoginRq)
        {
            return await _restClient.PostAsync<SystemLoginRq, SystemLoginRs>(
                "System",
                "login",
                systemLoginRq);
        }

        public async Task<IResultMonad<ExtendSystemSessionRs>> Extend(ExtendSystemSessionRq request)
        {
            return await _restClient.PostAsync<ExtendSystemSessionRq, ExtendSystemSessionRs>(
                "System",
                "extend-session",
                request);
        }
    }
}