using FluentValidation;
using Identity.Application.Users;

namespace Identity.Application.Users.UpdateUsers;

public sealed class UpdateUsersValidator : AbstractValidator<UpdateUsersCommand>
{
    public UpdateUsersValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

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

        RuleFor(x => x.Version).GreaterThan(0UL);
    }
}
