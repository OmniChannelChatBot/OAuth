namespace OAuth.Client
{
    public static class DBApiUrl
    {
        private static string UserPrefix { get; set; } = "user/";

        public static string Create { get; private set; } = "create";

        public static string Update { get; private set; } = "update";

        public static string CheckUserName { get; private set; } = "checkusername";

        public static string CheckUser { get; private set; } = "checkuser";

        public static string GetDBApiFullUrl (string baseUrl, string command)
        {
            return string.Concat(baseUrl, UserPrefix, command);
        }
    }
}
