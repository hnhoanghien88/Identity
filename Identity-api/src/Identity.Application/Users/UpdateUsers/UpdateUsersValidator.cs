using FluentValidation;

namespace Identity.Application.Users.UpdateUsers;

public sealed class UpdateUsersValidator : AbstractValidator<UpdateUsersCommand>
{
    public UpdateUsersValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
