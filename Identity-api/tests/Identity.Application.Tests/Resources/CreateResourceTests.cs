using FluentValidation;
using Identity.Application.Common.Exceptions;
using Identity.Application.Resources.CreateResource;

namespace Identity.Application.Tests.Resources;

public sealed class CreateResourceTests
{
    [Fact]
    public async Task Create_trims_fields_and_sets_scope_audit_and_version()
    {
        var repository = new TestResourcesRepository();
        var handler = new CreateResourceCommandHandler(
            repository,
            new CreateResourceValidator());

        var result = await handler.Handle(
            new CreateResourceCommand(
                1,
                " CODE ",
                " Name ",
                " Api ",
                " Description ",
                "admin"),
            CancellationToken.None);

        Assert.Equal("CODE", result.Code);
        Assert.Equal("Api", result.ResourceType);
        Assert.Equal(1UL, result.Version);
        Assert.Equal("admin", repository.Value!.CreatedBy);
    }

    [Fact]
    public async Task Create_rejects_unavailable_application_and_duplicate_code()
    {
        var unavailable = new TestResourcesRepository
        {
            ApplicationAvailable = false,
        };
        var firstHandler = new CreateResourceCommandHandler(
            unavailable,
            new CreateResourceValidator());
        var applicationError = await Assert.ThrowsAsync<ConflictException>(() =>
            firstHandler.Handle(
                new CreateResourceCommand(1, "R", "Name", "Api", null, null),
                CancellationToken.None));
        Assert.Equal("applicationId", applicationError.Field);

        var duplicate = new TestResourcesRepository
        {
            Duplicate = true,
        };
        var secondHandler = new CreateResourceCommandHandler(
            duplicate,
            new CreateResourceValidator());
        var codeError = await Assert.ThrowsAsync<ConflictException>(() =>
            secondHandler.Handle(
                new CreateResourceCommand(1, "R", "Name", "Api", null, null),
                CancellationToken.None));
        Assert.Equal("code", codeError.Field);
    }

    [Fact]
    public async Task Create_rejects_whitespace_and_overlength_values()
    {
        var handler = new CreateResourceCommandHandler(
            new TestResourcesRepository(),
            new CreateResourceValidator());

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(
                new CreateResourceCommand(1, " ", "Name", "Api", null, null),
                CancellationToken.None));
    }
}