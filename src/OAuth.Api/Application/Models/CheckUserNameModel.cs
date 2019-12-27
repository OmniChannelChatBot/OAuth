using System.ComponentModel.DataAnnotations;

namespace OAuth.Api.Application.Models
{
    public class CheckUserNameModel
    {
        [Required]
        public string UserName { get; set; }
    }
}
