using FluentValidation;

namespace Identity.Application.Users.CreateUsers;

public sealed class CreateUsersValidator : AbstractValidator<CreateUsersCommand>
{
    public CreateUsersValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
