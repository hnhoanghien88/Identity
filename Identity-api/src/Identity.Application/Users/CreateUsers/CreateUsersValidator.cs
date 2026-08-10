using FluentValidation;
using Identity.Application.Users;

namespace Identity.Application.Users.CreateUsers;

public sealed class CreateUsersValidator : AbstractValidator<CreateUsersCommand>
{
    public CreateUsersValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(UserIdentityRules.CodeMaximumLength)
            .Must(UserIdentityRules.IsValidCode)
            .WithMessage("Code may contain only ASCII letters, digits, dot, underscore, or hyphen without spaces.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(UserIdentityRules.EmailMaximumLength);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128);
    }
}
