using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OAuth.Entity;
using OAuth.Exception;
using OAuth.Helper;
using OAuth.Model;
using OAuth.Service;

namespace OAuth.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        //private IMapper _mapper;
        //private readonly AppSettings _appSettings;

        public UserController(
            IUserService userService)
            //IMapper mapper,
            //IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            //_mapper = mapper;
            //_appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            // return basic user info and authentication token
            return Ok(new UserModel
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = user.Token
            });
        }

        //[AllowAnonymous]
        //[HttpPost("register")]
        //public IActionResult Register([FromBody]RegisterModel model)
        //{
        //    // map model to entity
        //    var user = _mapper.Map<User>(model);

        //    try
        //    {
        //        // create user
        //        _userService.Create(user, model.Password);
        //        return Ok();
        //    }
        //    catch (OAuthException ex)
        //    {
        //        // return error message if there was an exception
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }
    }
}