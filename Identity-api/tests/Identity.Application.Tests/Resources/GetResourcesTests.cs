using Identity.Application.Abstractions.Persistence;
using Identity.Application.Resources.Dtos;
using Identity.Application.Resources.GetResources;
using Identity.Application.Users.GetUsers;

namespace Identity.Application.Tests.Resources;

public sealed class GetResourcesTests
{
    [Fact]
    public async Task Query_uses_default_page_and_deterministic_sort()
    {
        var repository = new CaptureRepository();
        await new GetResourcesQueryHandler(repository).Handle(
            new GetResourcesQuery(),
            CancellationToken.None);

        Assert.Equal(1, repository.Page);
        Assert.Equal(20, repository.PageSize);
        Assert.Collection(
            repository.Sorts,
            first =>
            {
                Assert.Equal(ResourcesSortColumn.CreatedDate, first.Column);
                Assert.Equal(SortDirection.Descending, first.Direction);
            },
            second => Assert.Equal(ResourcesSortColumn.Id, second.Column));
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public async Task Query_rejects_invalid_paging(int page, int pageSize)
    {
        var handler = new GetResourcesQueryHandler(new CaptureRepository());
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            handler.Handle(
                new GetResourcesQuery(Page: page, PageSize: pageSize),
                CancellationToken.None));
    }

    private sealed class CaptureRepository : IResourcesReadRepository
    {
        public IReadOnlyList<ResourcesSort> Sorts { get; private set; } = [];
        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public Task<ResourceDto?> GetByIdAsync(
            ulong id,
            CancellationToken cancellationToken) =>
            Task.FromResult<ResourceDto?>(null);

        public Task<PagedResourcesDto> GetAsync(
            ResourcesFilter filter,
            IReadOnlyList<ResourcesSort> sorts,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            Sorts = sorts;
            Page = page;
            PageSize = pageSize;
            return Task.FromResult(
                new PagedResourcesDto([], 0, page, pageSize));
        }
    }
}