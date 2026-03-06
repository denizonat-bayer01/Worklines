using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using wixi.Core.Configuration;
using System.Text;

namespace wixi.WebAPI.Extensions
{
    public static class JwtExt
    {
        public static IServiceCollection AddJwtExt(this IServiceCollection services, IConfiguration configuration)
        {
            var tokenOptions = configuration.GetSection("JwtTokenOptions").Get<JwtTokenOptions>();
            
            if (tokenOptions == null)
            {
                throw new InvalidOperationException("JwtTokenOptions configuration is missing or invalid.");
            }
            
            services.Configure<JwtTokenOptions>(configuration.GetSection("JwtTokenOptions"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey))
                };
            });

            return services;
        }
    }
}

