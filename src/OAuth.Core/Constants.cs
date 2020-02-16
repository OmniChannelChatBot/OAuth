namespace OAuth.Core
{
    public static class Constants
    {
        public static class JwtClaimIdentifiers
        {
            public const string Role = "rol", UserId = "id", RefreshTokenId = "rtid";
        }

        public static class JwtClaimValues
        {
            public const string ApiAccess = "api_access";
        }
    }
}
