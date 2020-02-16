using AutoMapper;
using MediatR;
using OAuth.Api.Application.Models;
using OAuth.Api.Application.Queries;
using OAuth.Infrastructure.Services;
using OCCBPackage.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth.Api.Application.QueryHandlers
{
    public class GetByUsernameQueryHandler : IRequestHandler<GetByUsernameQuery, GetByUsernameQueryResponse>
    {
        private readonly IDbApiServiceClient _dbApiServiceClient;
        private readonly IMapper _mapper;

        public GetByUsernameQueryHandler(IDbApiServiceClient dbApiServiceClient, IMapper mapper)
        {
            _dbApiServiceClient = dbApiServiceClient;
            _mapper = mapper;
        }

        public async Task<GetByUsernameQueryResponse> Handle(GetByUsernameQuery query, CancellationToken cancellationToken)
        {
            var user = await _dbApiServiceClient.FindUserByUsernameAsync(query.Username, cancellationToken);

            if (user == default)
            {
                throw new NotFoundException($"User {query.Username} not found");
            }

            return _mapper.Map<GetByUsernameQueryResponse>(user);
        }
    }
}
