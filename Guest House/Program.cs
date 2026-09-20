using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Guest_House.Data;
using Guest_House.DTOs.Common;
using Guest_House.Middleware;
using Guest_House.Services.Auth;
using Guest_House.Services.Password;
using Guest_House.Services.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace Guest_House
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Database Context Configuration
            var connectionString = builder.Configuration.GetConnectionString("dbConn")
                ?? "Server=localhost;Database=GuestHouse;Trusted_Connection=True;TrustServerCertificate=True;";
            builder.Services.AddDbContext<GuestHouseContext>(options =>
                options.UseSqlServer(connectionString));

            // 2. Controllers and Standardized Model Validation Responses
            builder.Services.AddControllers();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value != null && e.Value.Errors.Count > 0)
                        .SelectMany(e => e.Value!.Errors.Select(err => err.ErrorMessage))
                        .ToList();

                    var errorResponse = ApiResponse<object>.ErrorResponse("Validation failed.", errors);
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            // 3. Razor Pages (Preserved from existing project setup)
            builder.Services.AddRazorPages();

            // 4. Dependency Injection - Core Services
            builder.Services.AddScoped<IPasswordService, PasswordService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            // 5. JWT Authentication & Role Authorization Setup
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSection["Key"]
                ?? throw new InvalidOperationException("JWT Secret Key is not configured in appsettings.json.");
            var jwtIssuer = jwtSection["Issuer"];
            var jwtAudience = jwtSection["Audience"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name
                };

                // Handle token extraction dynamically to support both raw token and "Bearer <token>" input formats
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                        if (!string.IsNullOrEmpty(authHeader))
                        {
                            if (authHeader.StartsWith("Bearer Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                context.Token = authHeader.Substring(14).Trim();
                            }
                            else if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                context.Token = authHeader.Substring(7).Trim();
                            }
                            else
                            {
                                context.Token = authHeader.Trim();
                            }
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var response = ApiResponse<object>.ErrorResponse("Unauthorized. A valid JWT Bearer token is required to access this resource.");
                        await context.Response.WriteAsJsonAsync(response);
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        var response = ApiResponse<object>.ErrorResponse("Forbidden. You do not have the required role permissions to access this resource.");
                        await context.Response.WriteAsJsonAsync(response);
                    }
                };
            });

            builder.Services.AddAuthorization();

            // 6. Swagger / OpenAPI Configuration with Bearer Token Authorization
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Kalika Hotel & Lodge - Guest House Booking API",
                    Version = "v1",
                    Description = "RESTful Web API for Kalika Hotel & Lodge, Itahari-9, Buspark."
                });

                // Add JWT Bearer Security Definition ("Authorize" button in Swagger UI)
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter your JWT token (you can paste either raw token or 'Bearer <token>').",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", doc, null),
                        new List<string>()
                    }
                });
            });

            var app = builder.Build();

            // 7. HTTP Request Pipeline Configuration

            // A. Global Centralized Exception Handling Middleware
            app.UseMiddleware<ExceptionMiddleware>();

            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            // B. Swagger Documentation UI
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kalika Hotel & Lodge API v1");
                c.RoutePrefix = "swagger";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            // C. Authentication MUST run before Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // D. Endpoint Routing
            app.MapControllers();
            app.MapStaticAssets();
            app.MapRazorPages().WithStaticAssets();

            app.Run();
        }
    }
}
