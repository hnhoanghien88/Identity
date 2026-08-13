using Identity.Application.UserRoles;
namespace Identity.Application.Abstractions.Persistence;

public interface IUserRolesRepository
{
    Task<PagedUserRoleUsersDto> GetMembersAsync(ulong roleId, int page, int pageSize, CancellationToken cancellationToken);
    Task<PagedUserRoleUsersDto> GetCandidatesAsync(ulong roleId, string? search, int page, int pageSize, CancellationToken cancellationToken);
    Task<AssignUsersToRoleResultDto> AssignAsync(ulong roleId, IReadOnlyCollection<ulong> userIds, string? actor, CancellationToken cancellationToken);
    Task RemoveAsync(ulong roleId, ulong userId, CancellationToken cancellationToken);
}
