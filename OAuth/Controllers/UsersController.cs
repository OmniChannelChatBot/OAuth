using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OAuth.Api.Application.Entity;
using OAuth.Api.Application.Models;
using OAuth.Api.Application.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using System.Threading.Tasks;

namespace OAuth.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost]
        [SwaggerOperation(OperationId = nameof(Create))]
        [SwaggerResponse(StatusCodes.Status200OK, "Created")]
        public async Task<IActionResult> Create([FromBody, BindRequired]CreateUserModel model)
        {
            await _userService.CreateAsync(_mapper.Map<User>(model));
            return Ok();
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

        [HttpGet("{Id:int}")]
        [SwaggerOperation(OperationId = nameof(GetById))]
        [SwaggerResponse(StatusCodes.Status200OK, "User received", typeof(User))]
        public async Task<IActionResult> GetById([FromRoute]GetUserModel model)
        {
            var user = await _userService.GetAsync(model.Id);
            return Ok(user);
        }
    }
}