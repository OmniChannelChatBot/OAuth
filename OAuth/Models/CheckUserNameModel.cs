using System.ComponentModel.DataAnnotations;

namespace OAuth.Models
{
    public class CheckUserNameModel
    {
        [Required]
        public string UserName { get; set; }
    }
}
