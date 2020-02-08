namespace OAuth.Core.Options
{
    public class ClientOriginPolicyOptions
    {
        public string[] Origins { get; set; }

        public string[] Methods { get; set; }

        public string[] Headers { get; set; }

        public bool IsCredentials { get; set; }
    }
}
