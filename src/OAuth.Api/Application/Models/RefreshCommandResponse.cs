using System.ComponentModel.DataAnnotations;

namespace OAuth.Api.Application.Models
{
    public class RefreshCommandResponse
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
