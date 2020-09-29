namespace A19.User.Common
{
    public interface IPasswordHashing
    {
        /// <summary>
        ///     Used to hash a password and return the resulting string.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed string.</returns>
        string Hash(string password);

        /// <summary>
        ///     Used to verify a password.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="hashedPassword">The hash password to verify.</param>
        /// <returns>true if the password matches the hash.</returns>
        bool Verify(string password, string hashedPassword);
    }
}