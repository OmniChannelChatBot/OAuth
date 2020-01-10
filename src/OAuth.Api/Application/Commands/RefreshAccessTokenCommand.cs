using MediatR;
using OAuth.Api.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace OAuth.Api.Application.Commands
{
    public class RefreshAccessTokenCommand : IRequest<RefreshAccessTokenCommandResponse>
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
