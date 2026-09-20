using System.Security.Claims;
using System.Threading.Tasks;
using Guest_House.DTOs.Auth;
using Guest_House.DTOs.Common;
using Guest_House.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guest_House.Controllers
{
    /// <summary>
    /// Authentication and Staff User Identity Endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a staff user and returns a signed JWT token
        /// </summary>
        /// <param name="request">Staff username and password credentials</param>
        /// <response code="200">Login successful, returns token and safe user details</response>
        /// <response code="400">Validation error (e.g. missing username/password)</response>
        /// <response code="401">Invalid credentials or inactive account</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.IsSuccess)
            {
                return StatusCode(
                    result.StatusCode,
                    ApiResponse<object>.ErrorResponse(result.Message, result.Errors)
                );
            }

            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(result.Data!, result.Message));
        }

        /// <summary>
        /// Returns profile and claims of the currently authenticated staff user
        /// </summary>
        /// <response code="200">Authenticated user profile details</response>
        /// <response code="401">Missing or invalid JWT token</response>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("user_id")?.Value;
            var username = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirst("username")?.Value;
            var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirst("role")?.Value;
            var hotelId = User.FindFirst("hotel_id")?.Value;
            var fullName = User.FindFirst("full_name")?.Value;

            var profileData = new
            {
                userId = int.TryParse(userId, out var uid) ? uid : 0,
                hotelId = int.TryParse(hotelId, out var hid) ? hid : 0,
                username = username ?? string.Empty,
                fullName = fullName ?? string.Empty,
                role = role ?? string.Empty
            };

            return Ok(ApiResponse<object>.SuccessResponse(profileData, "Profile retrieved successfully."));
        }

        /// <summary>
        /// Protected endpoint accessible only to staff users with 'Admin' role
        /// </summary>
        /// <response code="200">Admin access verified</response>
        /// <response code="401">Missing or invalid JWT token</response>
        /// <response code="403">Authenticated but insufficient role permissions</response>
        [HttpGet("admin-only")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public IActionResult AdminOnly()
        {
            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                accessGranted = true,
                role = "Admin",
                message = "Welcome Administrator. You have full access to this endpoint."
            }, "Admin access verified."));
        }

        /// <summary>
        /// Protected endpoint accessible to staff users with 'Admin' or 'Manager' roles
        /// </summary>
        /// <response code="200">Managerial access verified</response>
        /// <response code="401">Missing or invalid JWT token</response>
        /// <response code="403">Authenticated but insufficient role permissions</response>
        [HttpGet("manager-only")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public IActionResult ManagerOnly()
        {
            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                accessGranted = true,
                role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirst("role")?.Value,
                message = "Managerial access granted."
            }, "Management access verified."));
        }
    }
}
