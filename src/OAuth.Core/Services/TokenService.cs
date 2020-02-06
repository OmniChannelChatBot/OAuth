using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth.Core.Dtos;
using OAuth.Core.Interfaces;
using OCCBPackage.Options;
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
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public TokenService(
            IOptions<AccessTokenOptions> accessTokenOptions,
            IOptions<RefreshTokenOptions> refreshTokenOptions)
        {
            _accessTokenOptions = accessTokenOptions?.Value ?? throw new ArgumentNullException(nameof(AccessTokenOptions));
            _refreshTokenOptions = refreshTokenOptions?.Value ?? throw new ArgumentNullException(nameof(RefreshTokenOptions));
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public AccessToken GenerateAccessToken(int userId, string username, int refreshTokenId)
        {
            var claimsIdentity = new ClaimsIdentity(
                new GenericIdentity(username, nameof(AccessToken.Token)),
                new Claim[]
                {
                    new Claim(Constants.JwtClaimIdentifiers.UserId, userId.ToString()),
                    new Claim(Constants.JwtClaimIdentifiers.RefreshTokenId, refreshTokenId.ToString()),
                    new Claim(Constants.JwtClaimIdentifiers.Role, Constants.JwtClaimValues.ApiAccess)
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
                    SecurityAlgorithms.HmacSha256)
            };

            var token = _jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);

            return new AccessToken
            {
                Token = _jwtSecurityTokenHandler.WriteToken(token),
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

        public ClaimsPrincipal GetClaimsPrincipalByExpiredAccessToken(string accessToken) =>
            _jwtSecurityTokenHandler.ValidateToken(
                accessToken,
                _accessTokenOptions.GetTokenValidationParameters(validateLifetime: false),
                out var _);

        public bool VerifyExpiredAccessToken(string accessToken)
        {
            try
            {
                _jwtSecurityTokenHandler.ValidateToken(
                    accessToken,
                    _accessTokenOptions.GetTokenValidationParameters(validateLifetime: false),
                    out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private long ToUnixEpochDate(DateTime date) =>
            (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}
