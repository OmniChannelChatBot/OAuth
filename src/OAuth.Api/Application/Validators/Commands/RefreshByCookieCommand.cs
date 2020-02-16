using FluentValidation;
using Microsoft.AspNetCore.Http;
using OAuth.Core.Interfaces;
using OAuth.Infrastructure.Services;
using OCCBPackage.Options;
using System;

namespace OAuth.Api.Application.Validators.Commands
{
    public class RefreshByCookieCommand : AbstractValidator<RefreshByCookieCommand>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbApiServiceClient _dbApiServiceClient;
        private readonly ITokenService _tokenService;

        public RefreshByCookieCommand(
            IHttpContextAccessor httpContextAccessor,
            IDbApiServiceClient dbApiServiceClient,
            ITokenService tokenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbApiServiceClient = dbApiServiceClient;
            _tokenService = tokenService;

            RuleFor(command => command)
                .Must(_ =>
                {
                    var accessToken = _httpContextAccessor.HttpContext.Request.Cookies[AccessTokenOptions.TokenName];
                    return _tokenService.VerifyExpiredAccessToken(accessToken);
                })
                .WithMessage("Invalid access token");
            RuleFor(command => command)
                .MustAsync(async (_, cancellationToken) =>
                {
                    var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies[RefreshTokenOptions.TokenName];
                    var result = await _dbApiServiceClient.FindRefreshTokenByTokenAsync(refreshToken, cancellationToken);

                    return result?.Token == refreshToken && result.Expires >= DateTimeOffset.UtcNow;
                })
                .WithMessage("Token not found or expired");
        }
    }
}
