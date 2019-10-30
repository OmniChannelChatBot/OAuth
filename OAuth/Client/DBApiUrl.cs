namespace OAuth.Client
{
    public static class DBApiUrl
    {
        private static string UserPrefix { get; set; } = "user/";

        public static string Create { get; private set; } = "create";

        public static string CheckUserName { get; private set; } = "checkusername";

        public static string GetDBApiFullUrl (string baseUrl, string command)
        {
            return string.Concat(baseUrl, UserPrefix, command);
        }
    }
}
