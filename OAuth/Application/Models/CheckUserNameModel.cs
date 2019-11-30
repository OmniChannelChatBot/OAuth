using System.ComponentModel.DataAnnotations;

namespace OAuth.Application.Models
{
    public class CheckUserNameModel
    {
        [Required]
        public string UserName { get; set; }
    }
}
