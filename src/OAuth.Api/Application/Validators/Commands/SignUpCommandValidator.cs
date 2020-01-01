using FluentValidation;
using FluentValidation.Validators;
using OAuth.Api.Application.Commands;
using OAuth.Api.Application.Models;
using OAuth.Infrastructure.Services;
using System;

namespace OAuth.Api.Application.Validators.Commands
{
    public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
    {
        private readonly IDbApiServiceClient _dbApiServiceClient;
        public SignUpCommandValidator(IDbApiServiceClient dbApiServiceClient)
        {
            _dbApiServiceClient = dbApiServiceClient;

            RuleFor(command => command)
               .Cascade(CascadeMode.StopOnFirstFailure)
               .NotNull()
               .WithMessage("Must not be null")
               .ChildRules(child =>
               {
                   child.RuleFor(command => command.FirstName)
                        .NotNull()
                        .WithMessage("Must not be null")
                        .NotEmpty()
                        .WithMessage("Should not be empty");
                   child.RuleFor(command => command.LastName)
                        .NotNull()
                        .WithMessage("Must not be null")
                        .NotEmpty()
                        .WithMessage("Should not be empty");
                   child.RuleFor(command => command.Username)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .NotNull()
                        .WithMessage("Must not be null")
                        .NotEmpty()
                        .WithMessage("Should not be empty")
                        .MustAsync(async (username, cancellationToken) =>
                        {
                            var available = await _dbApiServiceClient.AvailabilityUsernameAsync(username, cancellationToken);
                            return !available;
                        })
                        .WithMessage($"{nameof(SignUpCommand.Username)} is already in use");
                   child.RuleFor(command => command.Password)
                        .NotNull()
                        .WithMessage("Must not be null")
                        .NotEmpty()
                        .WithMessage("Should not be empty");
                   child.RuleFor(command => command.Email)
                        .NotNull()
                        .WithMessage("Must not be null")
                        .NotEmpty()
                        .WithMessage("Should not be empty")
                        .EmailAddress(EmailValidationMode.AspNetCoreCompatible)
                        .WithMessage("Do not match format");
                   child.RuleFor(command => command.Type)
                        .IsInEnum()
                        .WithMessage($"Must be one of {string.Join(',', Enum.GetNames(typeof(UserType)))}");
               });
        }
    }
}
