using System.ComponentModel.DataAnnotations;

namespace OAuth.Core.Dtos
{
    public class AccessToken
    {
        public string Token { get; set; }

        public int ExpiresIn { get; set; }
    }
}
