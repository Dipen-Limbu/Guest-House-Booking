using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.Auth
{
    /// <summary>
    /// Data transfer object for staff user login request
    /// </summary>
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}
