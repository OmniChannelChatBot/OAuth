﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OAuth.Application.Entity;
using OAuth.Application.Models;
using OAuth.Application.Services;
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
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        [SwaggerOperation(OperationId = nameof(Authenticate))]
        [SwaggerResponse(StatusCodes.Status200OK, "Authenticated", typeof(UserModel))]
        public async Task<IActionResult> Authenticate([FromBody, BindRequired]AuthenticateModel model)
        {
            var user = await _userService.AuthenticateAsync(model.Username, model.Password);

            if (user == default(User))
            {
                return NotFound("Username or password is incorrect");
            }


            return Ok(_mapper.Map<UserModel>(user));
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