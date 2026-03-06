using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using wixi.Core.Configuration;
using wixi.Entities.Concrete;
using wixi.Entities.Concrete.Identity;
using wixi.Entities.DTOs;

namespace wixi.Core.Utilities.Security.JWT
{
    public class JwtHelper : ITokenHelper
    {
        private readonly JwtTokenOptions _tokenOptions;
        private readonly UserManager<AppUser> _userManager;

        public JwtHelper(IOptions<JwtTokenOptions> tokenOptions, UserManager<AppUser> userManager)
        {
            _tokenOptions = tokenOptions.Value;
            _userManager = userManager;
        }

        public async Task<TokenDto> CreateToken(AppUser user)
        {
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOptions.RefreshTokenExpiration);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecurityKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var jwt = await CreateJwtSecurityToken(
                tokenOptions: _tokenOptions,
                signingCredentials: signingCredentials,
                user: user,
                expires: accessTokenExpiration
            );

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new TokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
                RefreshToken = CreateRefreshToken(),
                RefreshTokenExpiration = refreshTokenExpiration,
                Email = user.Email
            };
        }

        private async Task<JwtSecurityToken> CreateJwtSecurityToken(JwtTokenOptions tokenOptions,
            SigningCredentials signingCredentials, AppUser user, DateTime expires)
        {
            var jwt = new JwtSecurityToken(
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                expires: expires,
                notBefore: DateTime.UtcNow,
                claims: await SetClaims(user),
                signingCredentials: signingCredentials
            );
            return jwt;
        }

        private async Task<IEnumerable<Claim>> SetClaims(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("userId", user.Id.ToString()), // Add userId claim for controllers
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Kullanıcının rollerini al
            var userRoles = await _userManager.GetRolesAsync(user);
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            return claims;
        }

        private static string CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}

