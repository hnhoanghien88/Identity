using Identity.Application.Abstractions.Persistence;
using Identity.Application.Common.Exceptions;
using Identity.Application.Roles.Dtos;
using Identity.Application.Users.GetUsers;
using MediatR;

namespace Identity.Application.Roles.GetRoles;

public sealed record GetRolesQuery(
    RolesFilter? Filter = null,
    IReadOnlyList<RolesSort>? Sorts = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedRolesDto>;

public sealed record RolesFilter(
    IReadOnlyList<ulong>? ApplicationIds = null,
    StringFilter? Code = null,
    StringFilter? Name = null,
    bool? IsSystemRole = null,
    bool? IsActive = null);

public sealed record RolesSort(RolesSortColumn Column, SortDirection Direction = SortDirection.Ascending);

public enum RolesSortColumn { Id, Application, Code, Name, IsSystemRole, IsActive, CreatedDate }

public sealed class GetRolesQueryHandler(IRolesReadRepository repository)
    : IRequestHandler<GetRolesQuery, PagedRolesDto>
{
    public Task<PagedRolesDto> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(request.Page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(request.PageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(request.PageSize, 100);
        var sorts = request.Sorts is { Count: > 0 }
            ? request.Sorts
            : [
                new RolesSort(RolesSortColumn.CreatedDate, SortDirection.Descending),
                new RolesSort(RolesSortColumn.Id, SortDirection.Descending),
            ];
        return repository.GetAsync(
            request.Filter ?? new RolesFilter(),
            sorts,
            request.Page,
            request.PageSize,
            cancellationToken);
    }
}

public sealed record GetRoleByIdQuery(ulong Id) : IRequest<RoleDto>;

public sealed class GetRoleByIdQueryHandler(IRolesReadRepository repository)
    : IRequestHandler<GetRoleByIdQuery, RoleDto>
{
    public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken) =>
        await repository.GetByIdAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException($"Role '{request.Id}' was not found.");
}
