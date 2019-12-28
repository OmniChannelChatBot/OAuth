using AutoMapper;
using MediatR;
using OAuth.Api.Application.Commands;
using OAuth.Infrastructure.Services;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth.Api.Application.CommandHandlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
    {
        private readonly IDbApiServiceClient _dbApiServiceClient;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IDbApiServiceClient dbApiServiceClient, IMapper mapper)
        {
            _dbApiServiceClient = dbApiServiceClient;
            _mapper = mapper;
        }

        public Task<int> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var addUserCommand = _mapper.Map<AddUserCommand>(command);
            return _dbApiServiceClient.AddUserAsync(addUserCommand, cancellationToken);
        }
    }
}
