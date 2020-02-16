using System.ComponentModel.DataAnnotations;

namespace OAuth.Api.Application.Models
{
    public class SignInCommandResponse
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public UserType Type { get; set; }
    }
}
