using System.Collections.Generic;
using Guest_House.DTOs.Auth;
using Microsoft.AspNetCore.Http;

namespace Guest_House.Services.Auth
{
    /// <summary>
    /// Encapsulates authentication service execution outcome
    /// </summary>
    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public LoginResponseDto? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static AuthResult Success(LoginResponseDto data, string message = "Login successful.")
        {
            return new AuthResult
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = message,
                Data = data,
                Errors = new List<string>()
            };
        }

        public static AuthResult Failure(string message, int statusCode = StatusCodes.Status401Unauthorized, List<string>? errors = null)
        {
            return new AuthResult
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = message,
                Data = null,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
