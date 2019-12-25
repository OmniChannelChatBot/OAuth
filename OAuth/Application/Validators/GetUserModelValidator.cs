using FluentValidation;
using OAuth.Api.Application.Models;

namespace OAuth.Api.Application.Validators
{
    public class GetUserModelValidator: AbstractValidator<GetUserModel>
    {
        public GetUserModelValidator()
        {
            RuleFor(model => model.Id)
                .NotNull()
                .WithMessage("User ID not passed")
                .GreaterThanOrEqualTo(1)
                .WithMessage("User ID must be greater than zero");
        }
    }
}
