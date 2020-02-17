using MediatR;
using OAuth.Api.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace OAuth.Api.Application.Queries
{
    public class GetByUsernameQuery : IRequest<GetByUsernameQueryResponse>
    {
        [Required]
        public string Username { get; set; }
    }
}
