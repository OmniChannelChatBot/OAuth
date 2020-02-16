using MediatR;
using OAuth.Api.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace OAuth.Api.Application.Commands
{
    public class RefreshCommand : IRequest<RefreshCommandResponse>
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
