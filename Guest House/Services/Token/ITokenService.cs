using Guest_House.Models;

namespace Guest_House.Services.Token
{
    /// <summary>
    /// Service for generating JWT tokens for authenticated staff users
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a signed JWT token containing staff claims (id, username, role, hotel_id)
        /// </summary>
        /// <param name="user">Staff user entity with loaded Role navigation property</param>
        /// <returns>JWT Bearer token string</returns>
        string GenerateToken(StaffUser user);
    }
}
