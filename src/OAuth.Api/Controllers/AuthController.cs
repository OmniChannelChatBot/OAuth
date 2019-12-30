﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OAuth.Api.Application.Commands;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace OAuth.Api.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) =>
            _mediator = mediator;

        [HttpPost("sign-up")]
        [SwaggerOperation(OperationId = nameof(SignUpAsync))]
        [SwaggerResponse(StatusCodes.Status200OK, "Sign up")]
        public async Task<IActionResult> SignUpAsync([FromBody, BindRequired]SignUpCommand command)
        {
            var userId = await _mediator.Send(command);
            return Ok(userId);
        }

        [HttpPost("sign-in")]
        [SwaggerOperation(OperationId = nameof(SignInAsync))]
        [SwaggerResponse(StatusCodes.Status200OK, "Sign in")]
        public async Task<IActionResult> SignInAsync([FromBody, BindRequired]SignInCommand command)
        {
            var signInCommandResponse = await _mediator.Send(command);
            return Ok(signInCommandResponse);
        }
    }
}