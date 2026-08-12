using FluentValidation;

namespace Identity.Application.Applications.UpdateApplication;

public sealed class UpdateApplicationValidator : AbstractValidator<UpdateApplicationCommand>
{
    public UpdateApplicationValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0UL);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Audience).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Version).GreaterThan(0UL);
    }
}
