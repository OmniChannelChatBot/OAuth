using System;

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
    }
}
