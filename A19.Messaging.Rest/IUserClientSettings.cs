using A19.Core;

namespace A19.Messaging.Rest
{
    public interface IUserClientSettings : ISystemSettings
    {
        string UserLoginUrl { get;  }
    }
}