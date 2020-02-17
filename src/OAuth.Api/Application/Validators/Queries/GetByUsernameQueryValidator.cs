using FluentValidation;
using OAuth.Api.Application.Queries;

namespace OAuth.Api.Application.Validators.Queries
{
    public class GetByUsernameQueryValidator : AbstractValidator<GetByUsernameQuery>
    {
        public GetByUsernameQueryValidator()
        {
            RuleFor(query => query)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .ChildRules(child => child
                    .RuleFor(command => command.Username)
                        .NotNull()
                        .WithMessage("Must not be null")
                        .NotEmpty()
                        .WithMessage("Should not be empty")
                );
        }
    }
}
