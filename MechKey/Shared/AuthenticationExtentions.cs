using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Shared
{
    public static class AuthenticationExtentions
    {
        public static IServiceCollection AddAuthenticationConfig(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["accessToken"]; // read from cookie
                        if (!string.IsNullOrEmpty(token))
                            context.Token = token;

                        return Task.CompletedTask;
                    },
                    //OnChallenge = context =>
                    //{
                    //    // ✅ Trả JSON thay vì redirect nếu là API
                    //    context.HandleResponse();
                    //    context.Response.StatusCode = 401;
                    //    context.Response.ContentType = "application/json";
                    //    return context.Response.WriteAsync("{\"error\":\"Unauthorized\"}");
                    //},
                    //OnForbidden = context =>
                    //{
                    //    context.Response.StatusCode = 403;
                    //    context.Response.ContentType = "application/json";
                    //    return context.Response.WriteAsync("{\"error\":\"Forbidden\"}");
                    //}
                };
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = config.GetSection("JwtSettings:Issuer").Value!,
                    ValidAudience = config.GetSection("JwtSettings:Audience").Value!,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JwtSettings:Key").Value!))
                };
            });

            return services;
        }
    }
}
