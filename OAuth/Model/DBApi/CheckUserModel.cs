using System.ComponentModel.DataAnnotations;

namespace OAuth.Model.DBApi
{
    public class CheckUserModel
    {
        [Required]
        public string Username { get; set; }
    }
}
