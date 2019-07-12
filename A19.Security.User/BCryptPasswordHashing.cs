using A19.User.Common;

namespace A19.Security.User
{
    public class BCryptPasswordHashing : IPasswordHashing
    {

        private const int COST = 13;
        
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, COST);
        }

        public bool Verify(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}