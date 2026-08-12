using FluentValidation;

namespace Identity.Application.Applications.CreateApplication;

public sealed class CreateApplicationValidator : AbstractValidator<CreateApplicationCommand>
{
    public CreateApplicationValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Audience).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
