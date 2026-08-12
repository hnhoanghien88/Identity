using FluentValidation;

namespace Identity.Application.Menus.UpdateMenu;

public sealed class UpdateMenuValidator : AbstractValidator<UpdateMenuCommand>
{
    public UpdateMenuValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0UL);
        RuleFor(x => x.ApplicationId).GreaterThan(0UL);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Route).MaximumLength(300);
        RuleFor(x => x.Icon).MaximumLength(100);
        RuleFor(x => x.Version).GreaterThan(0UL);
    }
}
