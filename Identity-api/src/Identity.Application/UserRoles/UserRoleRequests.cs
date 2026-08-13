using Identity.Application.Abstractions.Persistence;
using MediatR;
namespace Identity.Application.UserRoles;

public sealed record UserRoleUserDto(ulong UserId, string Code, string Name, string Email);
public sealed record PagedUserRoleUsersDto(IReadOnlyList<UserRoleUserDto> Items, int Page, int PageSize, int TotalCount);
public sealed record AssignUsersToRoleResultDto(ulong RoleId, IReadOnlyList<ulong> AssignedUserIds, IReadOnlyList<ulong> AlreadyAssignedUserIds);
public sealed record GetRoleMembersQuery(ulong RoleId, int Page = 1, int PageSize = 20) : IRequest<PagedUserRoleUsersDto>;
public sealed class GetRoleMembersQueryHandler(IUserRolesRepository repository) : IRequestHandler<GetRoleMembersQuery, PagedUserRoleUsersDto>
{
    public Task<PagedUserRoleUsersDto> Handle(GetRoleMembersQuery r, CancellationToken ct) { Validate(r.RoleId, r.Page, r.PageSize); return repository.GetMembersAsync(r.RoleId, r.Page, r.PageSize, ct); }
    internal static void Validate(ulong roleId, int page, int pageSize) { ArgumentOutOfRangeException.ThrowIfZero(roleId); if (page < 1 || pageSize is < 1 or > 100) throw new ArgumentOutOfRangeException(nameof(page)); }
}
public sealed record GetRoleCandidatesQuery(ulong RoleId, string? Search, int Page = 1, int PageSize = 20) : IRequest<PagedUserRoleUsersDto>;
public sealed class GetRoleCandidatesQueryHandler(IUserRolesRepository repository) : IRequestHandler<GetRoleCandidatesQuery, PagedUserRoleUsersDto>
{
    public Task<PagedUserRoleUsersDto> Handle(GetRoleCandidatesQuery r, CancellationToken ct) { GetRoleMembersQueryHandler.Validate(r.RoleId, r.Page, r.PageSize); if (r.Search?.Length > 100) throw new ArgumentException("Search cannot exceed 100 characters."); return repository.GetCandidatesAsync(r.RoleId, r.Search?.Trim(), r.Page, r.PageSize, ct); }
}
public sealed record AssignUsersToRoleCommand(ulong RoleId, IReadOnlyCollection<ulong> UserIds, string? Actor) : IRequest<AssignUsersToRoleResultDto>;
public sealed class AssignUsersToRoleCommandHandler(IUserRolesRepository repository) : IRequestHandler<AssignUsersToRoleCommand, AssignUsersToRoleResultDto>
{
    public Task<AssignUsersToRoleResultDto> Handle(AssignUsersToRoleCommand r, CancellationToken ct) { ArgumentOutOfRangeException.ThrowIfZero(r.RoleId); if (r.UserIds.Count is < 1 or > 100 || r.UserIds.Any(id => id == 0) || r.UserIds.Distinct().Count() != r.UserIds.Count) throw new ArgumentException("Provide between 1 and 100 unique positive userIds."); return repository.AssignAsync(r.RoleId, r.UserIds, r.Actor, ct); }
}
public sealed record RemoveUserFromRoleCommand(ulong RoleId, ulong UserId) : IRequest;
public sealed class RemoveUserFromRoleCommandHandler(IUserRolesRepository repository) : IRequestHandler<RemoveUserFromRoleCommand>
{
    public Task Handle(RemoveUserFromRoleCommand r, CancellationToken ct) { ArgumentOutOfRangeException.ThrowIfZero(r.RoleId); ArgumentOutOfRangeException.ThrowIfZero(r.UserId); return repository.RemoveAsync(r.RoleId, r.UserId, ct); }
}
