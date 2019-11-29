using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAuth.Entity;
using OAuth.Exceptions;
using OAuth.Models;
using OAuth.Services;
using System.Threading.Tasks;

namespace OAuth.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService) =>
            _userService = userService;

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateModel model)
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
        [HttpPost("checkusername")]
        public async Task<IActionResult> CheckUserNameAsync([FromBody]CheckUserNameModel model)
        {
            var exists = await _userService.CheckUserNameAsync(model.UserName);

            return Ok(exists);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var user = await _userService.GetAsync(id);
            return Ok(user);
        }
    }
}