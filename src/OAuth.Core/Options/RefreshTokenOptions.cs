using System;

namespace OAuth.Core.Options
{
    public class RefreshTokenOptions
    {
        public DateTimeOffset Expires => DateTimeOffset.UtcNow.AddDays(ExpiresInDays);

        public double ExpiresInDays { get; set; } = 5.0;
    }
}
