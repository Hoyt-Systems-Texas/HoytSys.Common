using System.Threading.Tasks;
using A19.Database.Repository.A19Test.User;
using A19.Security.User;
using A19.User.Common;

namespace TestApp.User
{
    public class UserService : IUserService
    {

        private readonly IUserRepository userRepository;
        private readonly IPasswordHashing passwordHashing;

        public UserService(
            IUserRepository userRepository,
            IPasswordHashing passwordHashing)
        {
            this.userRepository = userRepository;
            this.passwordHashing = passwordHashing;
        }
        
        public async Task<UserLoginRs> Login(UserLoginRq userLoginRq)
        {
            var user = await this.userRepository.Get(userLoginRq.Username);
            if (user == null)
            {
                return new UserLoginRs
                {
                    Success = false
                };
            }
            else
            {
                if (this.passwordHashing.Verify(
                    userLoginRq.Password,
                    user.Password))
                {
                    return new UserLoginRs
                    {
                        Success = true,
                        UserGuid = user.UserGuid
                    };
                }
                else
                {
                    return new UserLoginRs()
                    {
                        Success = false
                    };
                }
            }
        }
    }
}