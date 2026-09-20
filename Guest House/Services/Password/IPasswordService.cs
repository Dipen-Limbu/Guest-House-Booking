namespace Guest_House.Services.Password
{
    /// <summary>
    /// Service for securely hashing and verifying passwords
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Hashes a plain-text password using a cryptographic salt and PBKDF2
        /// </summary>
        /// <param name="password">Plain-text password</param>
        /// <returns>Formatted password hash string</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a plain-text password against a stored password hash
        /// </summary>
        /// <param name="password">Plain-text password to check</param>
        /// <param name="passwordHash">Stored password hash</param>
        /// <returns>True if password matches; otherwise false</returns>
        bool VerifyPassword(string password, string passwordHash);
    }
}
