using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Guest_House.DTOs.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Guest_House.Middleware
{
    /// <summary>
    /// Centralized exception handling middleware to catch unhandled errors,
    /// log detailed diagnostics server-side, and return a clean, consistent API response
    /// without leaking sensitive internals or stack traces.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during request {Path}: {Message}",
                    context.Request.Path, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, exception.Message),
                ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
                InvalidOperationException => (StatusCodes.Status400BadRequest, exception.Message),
                _ => (StatusCodes.Status500InternalServerError, _env.IsDevelopment() ? exception.GetBaseException().Message : "An unexpected error occurred.")
            };

            context.Response.StatusCode = statusCode;

            var errorResponse = new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Data = null,
                Errors = new List<string>()
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
