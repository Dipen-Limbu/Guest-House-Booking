namespace Guest_House.DTOs.Auth
{
    /// <summary>
    /// Data transfer object for staff user login response
    /// </summary>
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserInfoDto User { get; set; } = null!;
    }

    /// <summary>
    /// Safe public representation of staff user details (no sensitive fields)
    /// </summary>
    public class UserInfoDto
    {
        public int UserId { get; set; }
        public int HotelId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
