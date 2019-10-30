using System.ComponentModel.DataAnnotations;

namespace OAuth.Model
{
    public class CheckUserModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
