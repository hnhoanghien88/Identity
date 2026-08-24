using FluentValidation;
using Identity.Application.Common.Behaviors;

namespace Identity.Application.Tests.Common.Behaviors;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_calls_next_when_no_validator_is_registered()
    {
        var behavior = new ValidationBehavior<TestRequest, string>([]);
        var nextCalled = false;

        var result = await behavior.Handle(
            new TestRequest(""),
            _ =>
            {
                nextCalled = true;
                return Task.FromResult("handled");
            },
            CancellationToken.None);

        Assert.True(nextCalled);
        Assert.Equal("handled", result);
    }

    [Fact]
    public async Task Handle_aggregates_failures_and_does_not_call_next()
    {
        IValidator<TestRequest>[] validators =
        [
            new RequiredValueValidator(),
            new MinimumLengthValidator(),
        ];
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var nextCalled = false;

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(
                new TestRequest(""),
                _ =>
                {
                    nextCalled = true;
                    return Task.FromResult("handled");
                },
                CancellationToken.None));

        Assert.False(nextCalled);
        Assert.Equal(2, exception.Errors.Count());
    }

    [Fact]
    public async Task Handle_calls_next_after_successful_validation()
    {
        IValidator<TestRequest>[] validators = [new RequiredValueValidator()];
        var behavior = new ValidationBehavior<TestRequest, string>(validators);

        var result = await behavior.Handle(
            new TestRequest("valid"),
            _ => Task.FromResult("handled"),
            CancellationToken.None);

        Assert.Equal("handled", result);
    }

    private sealed record TestRequest(string Value);

    private sealed class RequiredValueValidator : AbstractValidator<TestRequest>
    {
        public RequiredValueValidator() => RuleFor(request => request.Value).NotEmpty();
    }

    private sealed class MinimumLengthValidator : AbstractValidator<TestRequest>
    {
        public MinimumLengthValidator() => RuleFor(request => request.Value).MinimumLength(3);
    }
}
