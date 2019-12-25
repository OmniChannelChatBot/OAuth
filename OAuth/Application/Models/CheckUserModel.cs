using System.ComponentModel.DataAnnotations;

namespace OAuth.Api.Application.Models
{
    public class CheckUserModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
