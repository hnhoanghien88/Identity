using FluentValidation;

namespace Identity.Application.Actions.UpdateAction;

public sealed class UpdateActionValidator : AbstractValidator<UpdateActionCommand>
{
    public UpdateActionValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0UL);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Version).GreaterThan(0UL);
    }
}

