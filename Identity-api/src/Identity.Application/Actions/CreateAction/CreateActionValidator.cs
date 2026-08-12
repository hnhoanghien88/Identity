using FluentValidation;

namespace Identity.Application.Actions.CreateAction;

public sealed class CreateActionValidator : AbstractValidator<CreateActionCommand>
{
    public CreateActionValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

