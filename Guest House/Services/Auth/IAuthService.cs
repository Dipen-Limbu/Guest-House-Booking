using System.Threading.Tasks;
using Guest_House.DTOs.Auth;

namespace Guest_House.Services.Auth
{
    /// <summary>
    /// Authentication service contract handling staff login and credentials validation
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates staff user credentials and generates a JWT Bearer token
        /// </summary>
        /// <param name="request">Username and password request DTO</param>
        /// <returns>AuthResult with status code, message, and LoginResponseDto</returns>
        Task<AuthResult> LoginAsync(LoginRequestDto request);
    }
}
