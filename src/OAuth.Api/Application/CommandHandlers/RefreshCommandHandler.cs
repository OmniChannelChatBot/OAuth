﻿using MediatR;
using Microsoft.AspNetCore.Http;
using OAuth.Api.Application.Commands;
using OAuth.Api.Application.Models;
using OAuth.Core.Interfaces;
using OAuth.Infrastructure.Services;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth.Api.Application.CommandHandlers
{
    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, RefreshCommandResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbApiServiceClient _dbApiServiceClient;
        private readonly ITokenService _tokenService;

        public RefreshCommandHandler(
            IHttpContextAccessor httpContextAccessor,
            IDbApiServiceClient dbApiServiceClient,
            ITokenService tokenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbApiServiceClient = dbApiServiceClient;
            _tokenService = tokenService;
        }

        public async Task<RefreshCommandResponse> Handle(RefreshCommand command, CancellationToken cancellationToken)
        {
            var claimsPrincipal = _tokenService.ValidateExpiredAccessToken(command.AccessToken) ??
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

            return new RefreshCommandResponse
            {
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
