namespace Identity.Application.Abstractions.Persistence;

public interface IUserRolesReadRepository
{
    Task<IReadOnlyList<string>> GetRoleCodesAsync(ulong userId, CancellationToken cancellationToken);
}
