using FluentValidation;

namespace Identity.Application.Menus.CreateMenu;

public sealed class CreateMenuValidator : AbstractValidator<CreateMenuCommand>
{
    public CreateMenuValidator()
    {
        RuleFor(x => x.ApplicationId).GreaterThan(0UL);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Route).MaximumLength(300);
        RuleFor(x => x.Icon).MaximumLength(100);
    }
}
