using System.Collections.Generic;

namespace Guest_House.DTOs.Common
{
    /// <summary>
    /// Unified API response contract for all API endpoints
    /// </summary>
    /// <typeparam name="T">Type of data returned in response payload</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public ApiResponse()
        {
        }

        public ApiResponse(bool success, string message, T? data = default, List<string>? errors = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
        }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Request successful.")
        {
            return new ApiResponse<T>(true, message, data);
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>(false, message, default, errors);
        }
    }

    /// <summary>
    /// Non-generic API response helper for operations without return data
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse SuccessResult(string message = "Request successful.")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                Data = null,
                Errors = new List<string>()
            };
        }

        public static ApiResponse ErrorResult(string message, List<string>? errors = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Data = null,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
