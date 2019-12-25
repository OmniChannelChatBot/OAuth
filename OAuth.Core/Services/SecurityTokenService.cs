using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth.Core.Interfaces;
using OAuth.Core.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OAuth.Core.Services
{
    public class SecurityTokenService : ISecurityTokenService
    {
        private readonly SecurityTokenOptions _securityTokenOptions;
        public SecurityTokenService(IOptions<SecurityTokenOptions> options) =>
            _securityTokenOptions = options?.Value ?? throw new ArgumentNullException(nameof(SecurityTokenOptions));
        public string GenerateAccessToken(ClaimsIdentity claimsIdentity)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _securityTokenOptions.Issuer,
                Audience = _securityTokenOptions.Audience,
                Subject = claimsIdentity,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(_securityTokenOptions.ExpiresInMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityTokenOptions.Secret)),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);

            return jwtSecurityTokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetClaimsPrincipalByExpiredToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityTokenOptions.Secret)),
                ValidateIssuer = true,
                ValidIssuer = _securityTokenOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _securityTokenOptions.Audience,
                ValidateLifetime = true,
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var principal = jwtSecurityTokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
