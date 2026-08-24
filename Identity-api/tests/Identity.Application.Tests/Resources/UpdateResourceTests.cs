using Identity.Application.Common.Exceptions;
using Identity.Application.Resources.UpdateResource;

namespace Identity.Application.Tests.Resources;

public sealed class UpdateResourceTests
{
    [Fact]
    public async Task Update_moves_scope_and_increments_version_once()
    {
        var repository = new TestResourcesRepository
        {
            Value = TestResourcesRepository.Existing(3),
        };
        var handler = new UpdateResourceCommandHandler(repository);

        var result = await handler.Handle(
            new UpdateResourceCommand(
                1,
                2,
                "NEW",
                "Changed",
                "Screen",
                null,
                false,
                3,
                "admin"),
            CancellationToken.None);

        Assert.Equal(2UL, result.ApplicationId);
        Assert.Equal(4UL, result.Version);
        Assert.False(result.IsActive);
        Assert.Equal(1, repository.SaveCount);
    }

    [Fact]
    public async Task Update_rejects_stale_version_without_saving()
    {
        var repository = new TestResourcesRepository
        {
            Value = TestResourcesRepository.Existing(2),
        };
        var handler = new UpdateResourceCommandHandler(repository);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(
                new UpdateResourceCommand(
                    1,
                    1,
                    "R",
                    "Name",
                    "Api",
                    null,
                    true,
                    1,
                    null),
                CancellationToken.None));

        Assert.Equal(0, repository.SaveCount);
    }
}