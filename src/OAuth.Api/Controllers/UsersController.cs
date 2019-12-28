using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OAuth.Api.Application.Commands;
using OAuth.Api.Application.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace OAuth.Api.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator) =>
            _mediator = mediator;

        [AllowAnonymous]
        [HttpPost]
        [SwaggerOperation(OperationId = nameof(CreateUserAsync))]
        [SwaggerResponse(StatusCodes.Status200OK, "Created")]
        public async Task<IActionResult> CreateUserAsync([FromBody, BindRequired]CreateUserCommand command)
        {
            var userId = await _mediator.Send(command);
            return Ok(userId);
        }

        //[AllowAnonymous]
        //[HttpPost("checkusername")]
        //[SwaggerOperation(OperationId = nameof(CheckUserNameAsync))]
        //[SwaggerResponse(StatusCodes.Status200OK, "User name checked", typeof(bool))]
        //public async Task<IActionResult> CheckUserNameAsync([FromBody]CheckUserNameModel model)
        //{
        //    var exists = await _userService.CheckUserNameAsync(model.UserName);

        //    return Ok(exists);
        //}

        //[HttpGet("{Id:int}")]
        //[SwaggerOperation(OperationId = nameof(GetById))]
        //[SwaggerResponse(StatusCodes.Status200OK, "User received", typeof(User))]
        //public async Task<IActionResult> GetById([FromRoute]GetUserModel model)
        //{
        //    var user = await _userService.GetAsync(model.Id);
        //    return Ok(user);
        //}
    }
}