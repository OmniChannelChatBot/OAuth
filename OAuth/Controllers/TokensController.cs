﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OAuth.Api.Application.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using System.Threading.Tasks;

namespace OAuth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class TokensController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public TokensController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        [SwaggerOperation(OperationId = nameof(Generate))]
        [SwaggerResponse(StatusCodes.Status200OK, "Token generated", typeof(UserModel))]
        public async Task<IActionResult> Generate([FromBody, BindRequired]AuthenticateModel model)
        {
            var user = await _userService.AuthenticateAsync(model.Username, model.Password);

            if (user == default(User))
            {
                return NotFound("Username or password is incorrect");
            }


            return Ok(_mapper.Map<UserModel>(user));
        }

        [HttpPut]
        [SwaggerOperation(OperationId = nameof(Refresh))]
        [SwaggerResponse(StatusCodes.Status200OK, "Token refreshed", typeof(UserModel))]
        public async Task<IActionResult> Refresh([FromBody, BindRequired]RefreshTokenModel model)
        {
            var principal = GetPrincipalByExpiredToken(token);
            var username = principal.Identity.Name;
            var savedRefreshToken = GetRefreshToken(username);
            if (savedRefreshToken != refreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            var newJwtToken = GenerateToken(principal.Claims);
            var newRefreshToken = GenerateRefreshToken();
            DeleteRefreshToken(username, refreshToken);
            SaveRefreshToken(username, newRefreshToken);

            return new ObjectResult(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken
            });
        }
    }
}