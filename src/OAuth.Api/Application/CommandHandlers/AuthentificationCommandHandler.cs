using MediatR;
using Microsoft.AspNetCore.Http;
using OAuth.Api.Application.Commands;
using OAuth.Api.Application.Models;
using OAuth.Core.Interfaces;
using OAuth.Infrastructure.Services;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth.Api.Application.CommandHandlers
{
    public class AuthentificationCommandHandler : IRequestHandler<AuthentificationCommand, AuthentificationCommandResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbApiServiceClient _dbApiServiceClient;
        private readonly ITokenService _tokenService;

        public AuthentificationCommandHandler(
            IHttpContextAccessor httpContextAccessor,
            IDbApiServiceClient dbApiServiceClient,
            ITokenService tokenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbApiServiceClient = dbApiServiceClient;
            _tokenService = tokenService;
        }

        public async Task<AuthentificationCommandResponse> Handle(AuthentificationCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbApiServiceClient.FindUserByUsernameAsync(command.Username, cancellationToken);
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

            return new AuthentificationCommandResponse
            {
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
