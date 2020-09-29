namespace A19.User.Common
{
    public interface IUserAuthenticateService
    {
        bool Authenticate(
            string username,
            string password);
    }
}