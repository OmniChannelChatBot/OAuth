using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAuth.Api.Application.Models;
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

        [HttpGet]
        [SwaggerOperation(OperationId = nameof(GetUserAsync))]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetUserQueryResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not found", typeof(ApiProblemDetails))]
        public async Task<IActionResult> GetUserAsync()
        {
            var query = new GetUserQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
