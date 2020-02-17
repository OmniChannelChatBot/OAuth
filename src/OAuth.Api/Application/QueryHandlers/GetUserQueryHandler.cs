using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using OAuth.Api.Application.Models;
using OAuth.Api.Application.Queries;
using OAuth.Infrastructure.Services;
using OCCBPackage.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth.Api.Application.QueryHandlers
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserQueryResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbApiServiceClient _dbApiServiceClient;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(
            IHttpContextAccessor httpContextAccessor,
            IDbApiServiceClient dbApiServiceClient,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbApiServiceClient = dbApiServiceClient;
            _mapper = mapper;
        }

        public async Task<GetUserQueryResponse> Handle(GetUserQuery query, CancellationToken cancellationToken)
        {
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(Core.Constants.JwtClaimIdentifiers.UserId).Value);
            var user = await _dbApiServiceClient.GetUserByIdAsync(userId, cancellationToken);

            if (user == default)
            {
                throw new NotFoundException($"User with id {userId} not found");
            }

            return _mapper.Map<GetUserQueryResponse>(user);
        }
    }
}
