using MediatR;
using OAuth.Api.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace OAuth.Api.Application.Commands
{
    public class CreateUserCommand : IRequest<int>
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public UserType Type { get; set; }
    }
}
