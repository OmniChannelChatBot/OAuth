using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OCCBPackage.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace OAuth.Api.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        [HttpGet]
        [SwaggerOperation(OperationId = nameof(Test))]
        [SwaggerResponse(StatusCodes.Status200OK, "Success")]
        public IActionResult Test() => Ok("Up");
    }
}
