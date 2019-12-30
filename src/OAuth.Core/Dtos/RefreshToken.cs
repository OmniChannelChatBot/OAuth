using System;

namespace OAuth.Core.Dtos
{
    public class RefreshToken
    {
        public string Token { get; set; }

        public DateTimeOffset Expires { get; set; }
    }
}
