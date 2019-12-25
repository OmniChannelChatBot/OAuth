namespace OAuth.Core.Options
{
    public class SecurityTokenOptions
    {
        public string Secret { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public double ExpiresInMinutes { get; set; }
    }
}
