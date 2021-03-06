﻿using MediatR;
using System.ComponentModel.DataAnnotations;

namespace OAuth.Api.Application.Commands
{
    public class SignInCommand : IRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
