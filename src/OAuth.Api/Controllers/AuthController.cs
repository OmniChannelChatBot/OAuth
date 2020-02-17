using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OAuth.Api.Application.Commands;
using OAuth.Api.Application.Models;
using OCCBPackage.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace OAuth.Api.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) =>
            _mediator = mediator;

        [HttpPost]
        [SwaggerOperation(OperationId = nameof(AuthentificationAsync))]
        [SwaggerResponse(StatusCodes.Status200OK, "Authentificated", typeof(AuthentificationCommandResponse))]
        public async Task<IActionResult> AuthentificationAsync([FromBody, BindRequired]AuthentificationCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("sign-in")]
        [SwaggerOperation(OperationId = nameof(SignInAsync))]
        [SwaggerResponse(StatusCodes.Status200OK, "Sign in")]
        public async Task<IActionResult> SignInAsync([FromBody, BindRequired]SignInCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("sign-up")]
        [SwaggerOperation(OperationId = nameof(SignUpAsync))]
        [SwaggerResponse(StatusCodes.Status200OK, "Sign up")]
        public async Task<IActionResult> SignUpAsync([FromBody, BindRequired]SignUpCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("refresh/cookie")]
        [SwaggerOperation(OperationId = nameof(RefreshByCookieAsync))]
        [SwaggerResponse(StatusCodes.Status200OK, "Refreshed")]
        public async Task<IActionResult> RefreshByCookieAsync()
        {
            var command = new RefreshByCookieCommand();
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("refresh")]
        [SwaggerOperation(OperationId = nameof(RefreshAsync))]
        [SwaggerResponse(StatusCodes.Status200OK, "Refreshed", typeof(RefreshCommandResponse))]
        public async Task<IActionResult> RefreshAsync([FromBody, BindRequired]RefreshCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}