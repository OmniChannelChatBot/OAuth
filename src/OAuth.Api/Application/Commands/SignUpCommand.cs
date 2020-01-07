using MediatR;
using OAuth.Api.Application.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OAuth.Api.Application.Commands
{
    public class SignUpCommand : IRequest<int>
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
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserType Type { get; set; }
    }
}
