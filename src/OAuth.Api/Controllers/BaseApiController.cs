using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace OAuth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public abstract class BaseApiController : ControllerBase
    {
    }
}
