using MediatR;
using OAuth.Api.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace OAuth.Api.Application.Commands
{
    public class SignInCommand : IRequest<SignInCommandResponse>
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
