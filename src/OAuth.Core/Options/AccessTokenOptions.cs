using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace OAuth.Core.Options
{
    public class AccessTokenOptions
    {
        public string Secret { get; set; }

        public string Issuer { get; set; }

        public DateTime IssuedAt => DateTime.UtcNow;

        public DateTime NotBefore => DateTime.UtcNow;

        public string Audience { get; set; }

        public DateTime Expires => IssuedAt.AddMinutes(ExpiresInMinutes);

        public double ExpiresInMinutes { get; set; } = 120.0;

        public TokenValidationParameters GetTokenValidationParameters(bool validateLifetime = true) =>
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),

                ValidateIssuer = true,
                ValidIssuer = Issuer,

                ValidateAudience = true,
                ValidAudience = Audience,

                ValidateLifetime = validateLifetime,

                ClockSkew = TimeSpan.FromSeconds(5)
            };
    }
}
