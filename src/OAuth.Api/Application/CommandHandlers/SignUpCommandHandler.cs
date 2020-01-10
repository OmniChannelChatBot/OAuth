using AutoMapper;
using MediatR;
using OAuth.Api.Application.Commands;
using OAuth.Core.Interfaces;
using OAuth.Infrastructure.Services;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth.Api.Application.CommandHandlers
{
    public class SignUpCommandHandler : IRequestHandler<SignUpCommand, int>
    {
        private readonly IDbApiServiceClient _dbApiServiceClient;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;

        public SignUpCommandHandler(IDbApiServiceClient dbApiServiceClient, IPasswordService passwordService, IMapper mapper)
        {
            _dbApiServiceClient = dbApiServiceClient;
            _passwordService = passwordService;
            _mapper = mapper;
        }

        public Task<int> Handle(SignUpCommand command, CancellationToken cancellationToken)
        {
            _passwordService.CreateHash(command.Password, out var hash, out var salt);

            var addUserCommand = _mapper.Map<AddUserCommand>(command, op =>
            {
                op.Items.Add(nameof(AddUserCommand.PasswordHash), hash);
                op.Items.Add(nameof(AddUserCommand.PasswordSalt), salt);
            });

            return _dbApiServiceClient.AddUserAsync(addUserCommand, cancellationToken);
        }
    }
}
