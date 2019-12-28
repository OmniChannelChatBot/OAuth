using FluentValidation;
using FluentValidation.Validators;
using OAuth.Api.Application.Commands;
using OAuth.Api.Application.Models;
using System;

namespace OAuth.Api.Application.Validators.Commands
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(command => command.FirstName)
                .NotNull()
                .WithMessage("Must not be null")
                .NotEmpty()
                .WithMessage("Should not be empty");
            RuleFor(command => command.LastName)
                .NotNull()
                .WithMessage("Must not be null")
                .NotEmpty()
                .WithMessage("Should not be empty");
            RuleFor(command => command.Username)
                .NotNull()
                .WithMessage("Must not be null")
                .NotEmpty()
                .WithMessage("Should not be empty");
            RuleFor(command => command.Password)
                .NotNull()
                .WithMessage("Must not be null")
                .NotEmpty()
                .WithMessage("Should not be empty");
            RuleFor(command => command.Email)
                .NotNull()
                .WithMessage("Must not be null")
                .NotEmpty()
                .WithMessage("Should not be empty")
                .EmailAddress(EmailValidationMode.AspNetCoreCompatible)
                .WithMessage("Do not match format");
            RuleFor(command => command.Type)
                .IsInEnum()
                .WithMessage($"Must be one of {string.Join(',', Enum.GetNames(typeof(UserType)))}");
        }
    }
}
