using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using OAuth.Api.Application.Commands;
using OAuth.Api.Application.Models;
using OAuth.Core.Interfaces;
using OAuth.Infrastructure.Services;
using OCCBPackage.Options;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth.Api.Application.CommandHandlers
{
    public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInCommandResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbApiServiceClient _dbApiServiceClient;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public SignInCommandHandler(
            IHttpContextAccessor httpContextAccessor,
            IDbApiServiceClient dbApiServiceClient,
            ITokenService tokenService,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbApiServiceClient = dbApiServiceClient;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<SignInCommandResponse> Handle(SignInCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbApiServiceClient.FindUserByUsernameAsync(command.Username);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var addRefreshTokenCommand = new AddRefreshTokenCommand
            {
                UserId = user.Id,
                Token = refreshToken.Token,
                Expires = refreshToken.Expires,
                RemoteIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()
            };

            var refreshTokenId = await _dbApiServiceClient.AddRefreshTokenAsync(addRefreshTokenCommand, cancellationToken);
            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Username, refreshTokenId);

            var result = _mapper.Map<SignInCommandResponse>(user);

            _httpContextAccessor.HttpContext.Response.Cookies.Append(AccessTokenOptions.TokenName, accessToken.Token);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(RefreshTokenOptions.TokenName, refreshToken.Token);

            return result;
        }
    }
}
