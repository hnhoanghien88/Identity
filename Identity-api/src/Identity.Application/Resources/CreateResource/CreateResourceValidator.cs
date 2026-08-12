using FluentValidation;

namespace Identity.Application.Resources.CreateResource;

public sealed class CreateResourceValidator : AbstractValidator<CreateResourceCommand>
{
    public CreateResourceValidator()
    {
        RuleFor(x => x.ApplicationId).GreaterThan(0UL);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ResourceType).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}