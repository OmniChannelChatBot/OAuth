using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OAuth.Entity;
using OAuth.Exceptions;
using OAuth.Models;
using OAuth.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using System.Threading.Tasks;

namespace OAuth.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService) =>
            _userService = userService;

        [AllowAnonymous]
        [HttpPost("authenticate")]
        [SwaggerOperation(OperationId = nameof(Authenticate))]
        [SwaggerResponse(StatusCodes.Status200OK, "Authenticated", typeof(UserModel))]
        public async Task<IActionResult> Authenticate([FromBody, BindRequired]AuthenticateModel model)
        {
            var user = await _userService.AuthenticateAsync(model.Username, model.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            // return basic user info and authentication token
            return Ok(new UserModel
            {
                Guid = user.Guid,
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = user.Token
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [SwaggerOperation(OperationId = nameof(Register))]
        [SwaggerResponse(StatusCodes.Status200OK, "Registered", typeof(RegisterModel))]
        public async Task<IActionResult> Register([FromBody, BindRequired]RegisterModel model)
        {
            // map model to entity
            //var user = _mapper.Map<User>(model);

            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Username = model.Username,
                Password = model.Password
            };

            try
            {
                // create user
                var responseUser = await _userService.CreateAsync(user);
                return Ok(responseUser);
            }
            catch (OAuthException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("checkusername")]
        [SwaggerOperation(OperationId = nameof(CheckUserNameAsync))]
        [SwaggerResponse(StatusCodes.Status200OK, "User name checked", typeof(bool))]
        public async Task<IActionResult> CheckUserNameAsync([FromBody]CheckUserNameModel model)
        {
            var exists = await _userService.CheckUserNameAsync(model.UserName);

            return Ok(exists);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(OperationId = nameof(GetAsync))]
        [SwaggerResponse(StatusCodes.Status200OK, "Users received", typeof(User))]
        public async Task<IActionResult> GetAsync(int id)
        {
            var user = await _userService.GetAsync(id);
            return Ok(user);
        }
    }
}