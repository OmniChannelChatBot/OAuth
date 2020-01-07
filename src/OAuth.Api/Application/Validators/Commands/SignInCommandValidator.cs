using FluentValidation;
using OAuth.Api.Application.Commands;
using OAuth.Core.Interfaces;
using OAuth.Infrastructure.Services;

namespace OAuth.Api.Application.Validators.Commands
{
    public class SignInCommandValidator : AbstractValidator<SignInCommand>
    {
        private readonly IDbApiServiceClient _dbApiServiceClient;
        private readonly IPasswordService _passwordService;

        public SignInCommandValidator(IDbApiServiceClient dbApiServiceClient, IPasswordService passwordService)
        {
            _dbApiServiceClient = dbApiServiceClient;
            _passwordService = passwordService;

            RuleFor(command => command)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .ChildRules(child =>
                {
                    child.RuleFor(command => command.Password)
                        .NotNull()
                        .WithMessage("Must not be null")
                        .NotEmpty()
                        .WithMessage("Should not be empty");
                    child.RuleFor(command => command.Username)
                        .NotNull()
                        .WithMessage("Must not be null")
                        .NotEmpty()
                        .WithMessage("Should not be empty");
                })
                .MustAsync(async (command, cancellationToken) =>
                {
                    // TODO: Пожалуй нужен cache, так еще есть вызов дальше в handler
                    var user = await _dbApiServiceClient.FindUserByUsernameAsync(command.Username, cancellationToken);
                    return user != default && _passwordService.Verify(command.Password, user.PasswordHash, user.PasswordSalt);
                })
                .WithMessage($"Invalid {nameof(SignInCommand.Username)} or {nameof(SignInCommand.Password)}");
        }
    }
}
