namespace A19.Security.User
{
    public interface IUserService
    {
        /// <summary>
        ///     Used to login a user.
        /// </summary>
        /// <param name="userLoginRq">The user login information.</param>
        /// <returns>The user login response.</returns>
        UserLoginRs Login(UserLoginRq userLoginRq);
    }
}