using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Guest_House.Services.Billing
{
    /// <summary>
    /// Encapsulates billing service operation execution outcomes with HTTP status codes and error messages
    /// </summary>
    public class BillingResult<T>
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static BillingResult<T> Success(T data, string message = "Operation successful.", int statusCode = StatusCodes.Status200OK)
        {
            return new BillingResult<T>
            {
                IsSuccess = true,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Errors = new List<string>()
            };
        }

        public static BillingResult<T> Failure(string message, int statusCode = StatusCodes.Status400BadRequest, List<string>? errors = null)
        {
            return new BillingResult<T>
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = message,
                Data = default,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
