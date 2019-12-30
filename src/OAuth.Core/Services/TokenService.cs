using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth.Core.Dtos;
using OAuth.Core.Interfaces;
using OAuth.Core.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace OAuth.Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly AccessTokenOptions _accessTokenOptions;
        private readonly RefreshTokenOptions _refreshTokenOptions;

        public TokenService(
            IOptions<AccessTokenOptions> accessTokenOptions,
            IOptions<RefreshTokenOptions> refreshTokenOptions)
        {
            _accessTokenOptions = accessTokenOptions?.Value ?? throw new ArgumentNullException(nameof(AccessTokenOptions));
            _refreshTokenOptions = refreshTokenOptions?.Value ?? throw new ArgumentNullException(nameof(RefreshTokenOptions));
        }

        public AccessToken GenerateAccessToken(int userId, string username)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var claimsIdentity = new ClaimsIdentity(
                new GenericIdentity(username, nameof(AccessToken.Token)),
                new Claim[]
                {
                    new Claim(Constants.JwtClaimIdentifiers.Id, userId.ToString()),
                    new Claim(Constants.JwtClaimIdentifiers.Rol, Constants.JwtClaims.ApiAccess)
                });

            var claims = new Dictionary<string, object>
            {
                { JwtRegisteredClaimNames.Sub, username },
                { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },
                { JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_accessTokenOptions.IssuedAt).ToString() },
             };

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _accessTokenOptions.Issuer,
                Audience = _accessTokenOptions.Audience,
                Subject = claimsIdentity,
                Claims = claims,
                NotBefore = _accessTokenOptions.NotBefore,
                IssuedAt = _accessTokenOptions.IssuedAt,
                Expires = _accessTokenOptions.Expires,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenOptions.Secret)),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);

            return new AccessToken
            {
                Token = jwtSecurityTokenHandler.WriteToken(token),
                ExpiresIn = (int)TimeSpan.FromMinutes(_accessTokenOptions.ExpiresInMinutes).TotalSeconds,
            };
        }

        public RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    Expires = _refreshTokenOptions.Expires
                };
            }
        }

        public ClaimsPrincipal GetClaimsPrincipalByExpiredToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenOptions.Secret)),
                ValidateIssuer = true,
                ValidIssuer = _accessTokenOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _accessTokenOptions.Audience,
                ValidateLifetime = true,
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var principal = jwtSecurityTokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private long ToUnixEpochDate(DateTime date) =>
            (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}
