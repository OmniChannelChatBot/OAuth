using FluentValidation;
using OAuth.Api.Application.Commands;
using OAuth.Core.Interfaces;
using OAuth.Infrastructure.Services;
using System;

namespace OAuth.Api.Application.Validators.Commands
{
    public class RefreshCommandValidator : AbstractValidator<RefreshCommand>
    {
        private readonly IDbApiServiceClient _dbApiServiceClient;
        private readonly ITokenService _tokenService;

        public RefreshCommandValidator(IDbApiServiceClient dbApiServiceClient, ITokenService tokenService)
        {
            _dbApiServiceClient = dbApiServiceClient;
            _tokenService = tokenService;

            RuleFor(command => command.AccessToken)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage("Must not be null")
                .NotEmpty()
                .WithMessage("Should not be empty")
                .Must(accessToken => _tokenService.VerifyExpiredAccessToken(accessToken))
                .WithMessage("Invalid access token");
            RuleFor(command => command.RefreshToken)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage("Must not be null")
                .NotEmpty()
                .WithMessage("Should not be empty")
                .MustAsync(async (refreshToken, cancellationToken) =>
                {
                    var result = await _dbApiServiceClient.FindRefreshTokenByTokenAsync(refreshToken, cancellationToken);
                    return result?.Token == refreshToken && result.Expires >= DateTimeOffset.UtcNow;
                })
                .WithMessage("Token not found or expired");
        }
    }
}
