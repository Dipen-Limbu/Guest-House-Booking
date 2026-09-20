using System.Threading.Tasks;
using Guest_House.Data;
using Guest_House.DTOs.Auth;
using Guest_House.Services.Password;
using Guest_House.Services.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Guest_House.Services.Auth
{
    /// <summary>
    /// Implements staff authentication flow using EF Core GuestHouseContext
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly GuestHouseContext _context;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            GuestHouseContext context,
            IPasswordService passwordService,
            ITokenService tokenService,
            ILogger<AuthService> logger)
        {
            _context = context;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<AuthResult> LoginAsync(LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return AuthResult.Failure("Username and password are required.", StatusCodes.Status400BadRequest);
            }

            // 1. Find the staff user by username and 5. Load the user's role
            var user = await _context.StaffUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            // 2. Check whether the user exists
            if (user == null)
            {
                _logger.LogWarning("Authentication failed: User '{Username}' does not exist.", request.Username);
                return AuthResult.Failure("Invalid username or password.", StatusCodes.Status401Unauthorized);
            }

            // 3. Check whether is_active is true
            if (!user.IsActive)
            {
                _logger.LogWarning("Authentication failed: User '{Username}' is inactive.", request.Username);
                return AuthResult.Failure("Account is inactive. Please contact your administrator.", StatusCodes.Status401Unauthorized);
            }

            // 4. Verify the supplied password against password_hash
            var isPasswordValid = _passwordService.VerifyPassword(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Authentication failed: Password mismatch for user '{Username}'.", request.Username);
                return AuthResult.Failure("Invalid username or password.", StatusCodes.Status401Unauthorized);
            }

            // Ensure role is loaded
            if (user.Role == null)
            {
                _logger.LogError("Authentication error: Role missing for user '{Username}'.", request.Username);
                return AuthResult.Failure("User role configuration error.", StatusCodes.Status500InternalServerError);
            }

            // 6. Generate a JWT token
            var token = _tokenService.GenerateToken(user);

            // 7. Return appropriate user information and the token (Excludes password/password_hash)
            var responseData = new LoginResponseDto
            {
                Token = token,
                User = new UserInfoDto
                {
                    UserId = user.UserId,
                    HotelId = user.HotelId,
                    Username = user.Username,
                    FullName = user.FullName,
                    Role = user.Role.RoleName
                }
            };

            _logger.LogInformation("Staff user '{Username}' successfully logged in.", user.Username);
            return AuthResult.Success(responseData, "Login successful.");
        }
    }
}
