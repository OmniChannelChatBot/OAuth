using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OAuth.Api.Application.Queries;
using OCCBPackage.Mvc;
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

        [HttpGet("username/{Username}")]
        [SwaggerOperation(OperationId = nameof(GetByUsernameAsync))]
        [SwaggerResponse(StatusCodes.Status200OK, "Success")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not found", typeof(ApiProblemDetails))]
        public async Task<IActionResult> GetByUsernameAsync([FromRoute, BindRequired]GetByUsernameQuery query)
        {
            var response = await _mediator.Send(query);

            return Ok(response);
        }
    }
}
