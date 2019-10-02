using A19.Messaging.Rest;

namespace A19.User.Rest.Test
{
    public class UserClientSettingsTest : IUserClientSettings
    {
        public string UserLoginUrl => "https://localhost:5002/api";
    }
}