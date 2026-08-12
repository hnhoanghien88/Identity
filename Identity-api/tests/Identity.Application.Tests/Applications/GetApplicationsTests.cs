using Identity.Application.Abstractions.Persistence;
using Identity.Application.Applications.Dtos;
using Identity.Application.Applications.GetApplications;
using Identity.Application.Users.GetUsers;

namespace Identity.Application.Tests.Applications;

public sealed class GetApplicationsTests
{
    [Fact]
    public async Task Query_uses_default_sort_and_page()
    {
        var repository = new CaptureRepository();
        var handler = new GetApplicationsQueryHandler(repository);

        await handler.Handle(new GetApplicationsQuery(), CancellationToken.None);

        Assert.Equal(1, repository.Page);
        Assert.Equal(20, repository.PageSize);
        Assert.Collection(
            repository.Sorts,
            first =>
            {
                Assert.Equal(ApplicationsSortColumn.CreatedDate, first.Column);
                Assert.Equal(SortDirection.Descending, first.Direction);
            },
            second => Assert.Equal(ApplicationsSortColumn.Id, second.Column));
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public async Task Query_rejects_invalid_paging(int page, int pageSize)
    {
        var handler = new GetApplicationsQueryHandler(new CaptureRepository());
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            handler.Handle(
                new GetApplicationsQuery(Page: page, PageSize: pageSize),
                CancellationToken.None));
    }

    private sealed class CaptureRepository : IApplicationsReadRepository
    {
        public IReadOnlyList<ApplicationsSort> Sorts { get; private set; } = [];
        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public Task<ApplicationDto?> GetByIdAsync(
            ulong id,
            CancellationToken cancellationToken) =>
            Task.FromResult<ApplicationDto?>(null);

        public Task<PagedApplicationsDto> GetAsync(
            ApplicationsFilter filter,
            IReadOnlyList<ApplicationsSort> sorts,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            Sorts = sorts;
            Page = page;
            PageSize = pageSize;
            return Task.FromResult(new PagedApplicationsDto([], 0, page, pageSize));
        }
    }
}
