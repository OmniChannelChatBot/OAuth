using MediatR;
using Microsoft.AspNetCore.Http;
using OAuth.Api.Application.Commands;
using OAuth.Core.Interfaces;
using OAuth.Infrastructure.Services;
using OCCBPackage.Options;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth.Api.Application.CommandHandlers
{
    public class RefreshByCookieCommandHandler : AsyncRequestHandler<RefreshByCookieCommand>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbApiServiceClient _dbApiServiceClient;
        private readonly ITokenService _tokenService;

        public RefreshByCookieCommandHandler(
            IHttpContextAccessor httpContextAccessor,
            IDbApiServiceClient dbApiServiceClient,
            ITokenService tokenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbApiServiceClient = dbApiServiceClient;
            _tokenService = tokenService;
        }

        protected override async Task Handle(RefreshByCookieCommand command, CancellationToken cancellationToken)
        {
            var currentAccessToken = _httpContextAccessor.HttpContext.Request.Cookies[AccessTokenOptions.TokenName];

            var claimsPrincipal = _tokenService.ValidateExpiredAccessToken(currentAccessToken) ??
                throw new InvalidOperationException($"{nameof(ClaimsPrincipal)} is null");

            var username = claimsPrincipal.Identity.Name;
            var userId = int.Parse(claimsPrincipal.FindFirst(Core.Constants.JwtClaimIdentifiers.UserId).Value);
            var refreshTokenId = int.Parse(claimsPrincipal.FindFirst(Core.Constants.JwtClaimIdentifiers.RefreshTokenId).Value);
            var remoteIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            var refreshToken = _tokenService.GenerateRefreshToken();

            var addRefreshTokenCommand = new AddRefreshTokenCommand
            {
                UserId = userId,
                Token = refreshToken.Token,
                Expires = refreshToken.Expires,
                RemoteIpAddress = remoteIpAddress
            };
            var addRefreshTokenTask = _dbApiServiceClient.AddRefreshTokenAsync(addRefreshTokenCommand, cancellationToken);

            var updateRefreshTokenCommand = new UpdateRefreshTokenCommand
            {
                Id = refreshTokenId,
                Expires = DateTimeOffset.UtcNow,
                RemoteIpAddress = remoteIpAddress
            };
            var updateRefreshTokenTask = _dbApiServiceClient.UpdateRefreshTokenAsync(updateRefreshTokenCommand, cancellationToken);

            await Task.WhenAll(addRefreshTokenTask, updateRefreshTokenTask);

            var accessToken = _tokenService.GenerateAccessToken(userId, username, await addRefreshTokenTask);

            _httpContextAccessor.HttpContext.Response.Cookies.Append(AccessTokenOptions.TokenName, accessToken.Token);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(RefreshTokenOptions.TokenName, refreshToken.Token);
        }
    }
}
