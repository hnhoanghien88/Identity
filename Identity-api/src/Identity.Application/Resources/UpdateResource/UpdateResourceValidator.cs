using FluentValidation;

namespace Identity.Application.Resources.UpdateResource;

public sealed class UpdateResourceValidator : AbstractValidator<UpdateResourceCommand>
{
    public UpdateResourceValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0UL);
        RuleFor(x => x.ApplicationId).GreaterThan(0UL);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ResourceType).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Version).GreaterThan(0UL);
    }
}