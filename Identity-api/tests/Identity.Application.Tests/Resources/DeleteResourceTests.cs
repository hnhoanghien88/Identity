using Identity.Application.Common.Exceptions;
using Identity.Application.Resources.DeleteResource;

namespace Identity.Application.Tests.Resources;

public sealed class DeleteResourceTests
{
    [Fact]
    public async Task Delete_is_blocked_by_dependencies()
    {
        var repository = new TestResourcesRepository
        {
            Value = TestResourcesRepository.Existing(),
            DependenciesExist = true,
        };

        await Assert.ThrowsAsync<ConflictException>(() =>
            new DeleteResourceCommandHandler(repository).Handle(
                new DeleteResourceCommand(1, 1, "admin"),
                CancellationToken.None));

        Assert.False(repository.Value!.IsDeleted);
    }

    [Fact]
    public async Task Delete_soft_deletes_and_increments_version()
    {
        var repository = new TestResourcesRepository
        {
            Value = TestResourcesRepository.Existing(),
        };

        await new DeleteResourceCommandHandler(repository).Handle(
            new DeleteResourceCommand(1, 1, "admin"),
            CancellationToken.None);

        Assert.True(repository.Value!.IsDeleted);
        Assert.False(repository.Value.IsActive);
        Assert.Equal(2UL, repository.Value.Version);
        Assert.Equal("admin", repository.Value.UpdatedBy);
    }
}