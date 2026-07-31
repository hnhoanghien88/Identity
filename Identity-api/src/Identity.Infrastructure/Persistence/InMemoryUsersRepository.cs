using System.Collections.Concurrent;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Users.GetUsers;
using UsersEntity = Identity.Domain.Entities.Users;

namespace Identity.Infrastructure.Persistence;

public sealed class InMemoryUsersRepository : IUsersRepository
{
    private readonly ConcurrentDictionary<Guid, UsersEntity> _users = [];

    public Task AddAsync(UsersEntity user, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (!_users.TryAdd(user.Id, user))
        {
            throw new InvalidOperationException("User already exists.");
        }

        return Task.CompletedTask;
    }

    public Task<UsersEntity?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _users.TryGetValue(id, out var user);

        return Task.FromResult(user);
    }

    public Task<bool> CodeExistsAsync(string code, Guid? excludingId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var exists = _users.Values.Any(user =>
            user.Id != excludingId &&
            string.Equals(user.Code, code.Trim(), StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }

    public Task UpdateAsync(UsersEntity user, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _users[user.Id] = user;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(UsersEntity user, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _users.TryRemove(user.Id, out _);

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<UsersEntity>> GetAsync(
        UsersFilter filter,
        IReadOnlyList<UsersSort> sorts,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        IEnumerable<UsersEntity> query = _users.Values;

        if (filter.Ids is { Count: > 0 })
        {
            query = query.Where(user => filter.Ids.Contains(user.Id));
        }

        query = Apply(query, user => user.Code, filter.Code);
        query = Apply(query, user => user.Name, filter.Name);

        if (filter.CreatedDateFrom.HasValue)
        {
            query = query.Where(user => user.CreatedDate >= filter.CreatedDateFrom);
        }

        if (filter.CreatedDateTo.HasValue)
        {
            query = query.Where(user => user.CreatedDate <= filter.CreatedDateTo);
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(user => user.IsActive == filter.IsActive);
        }

        IOrderedEnumerable<UsersEntity>? ordered = null;

        foreach (var sort in sorts)
        {
            ordered = Order(ordered ?? query, sort, ordered is not null);
        }

        var users = (ordered ?? query.OrderBy(_ => 0))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult<IReadOnlyList<UsersEntity>>(users);
    }

    private static IEnumerable<UsersEntity> Apply(
        IEnumerable<UsersEntity> query,
        Func<UsersEntity, string> valueSelector,
        StringFilter? filter)
    {
        if (filter is null)
        {
            return query;
        }

        if (filter.Values is { Count: > 0 })
        {
            query = query.Where(user =>
                filter.Values.Contains(valueSelector(user), StringComparer.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filter.Contains))
        {
            query = query.Where(user =>
                valueSelector(user).Contains(filter.Contains, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filter.StartsWith))
        {
            query = query.Where(user =>
                valueSelector(user).StartsWith(filter.StartsWith, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filter.EndsWith))
        {
            query = query.Where(user =>
                valueSelector(user).EndsWith(filter.EndsWith, StringComparison.OrdinalIgnoreCase));
        }

        return query;
    }

    private static IOrderedEnumerable<UsersEntity> Order(
        IEnumerable<UsersEntity> query,
        UsersSort sort,
        bool then) => (sort.Column, sort.Direction, then) switch
        {
            (UsersSortColumn.Id, SortDirection.Ascending, false) => query.OrderBy(user => user.Id),
            (UsersSortColumn.Id, SortDirection.Descending, false) => query.OrderByDescending(user => user.Id),
            (UsersSortColumn.Code, SortDirection.Ascending, false) => query.OrderBy(user => user.Code),
            (UsersSortColumn.Code, SortDirection.Descending, false) => query.OrderByDescending(user => user.Code),
            (UsersSortColumn.Name, SortDirection.Ascending, false) => query.OrderBy(user => user.Name),
            (UsersSortColumn.Name, SortDirection.Descending, false) => query.OrderByDescending(user => user.Name),
            (UsersSortColumn.CreatedDate, SortDirection.Ascending, false) => query.OrderBy(user => user.CreatedDate),
            (UsersSortColumn.CreatedDate, SortDirection.Descending, false) => query.OrderByDescending(user => user.CreatedDate),
            (UsersSortColumn.IsActive, SortDirection.Ascending, false) => query.OrderBy(user => user.IsActive),
            (_, _, false) => query.OrderByDescending(user => user.IsActive),
            (UsersSortColumn.Id, SortDirection.Ascending, true) => ((IOrderedEnumerable<UsersEntity>)query).ThenBy(user => user.Id),
            (UsersSortColumn.Id, _, true) => ((IOrderedEnumerable<UsersEntity>)query).ThenByDescending(user => user.Id),
            (UsersSortColumn.Code, SortDirection.Ascending, true) => ((IOrderedEnumerable<UsersEntity>)query).ThenBy(user => user.Code),
            (UsersSortColumn.Code, _, true) => ((IOrderedEnumerable<UsersEntity>)query).ThenByDescending(user => user.Code),
            (UsersSortColumn.Name, SortDirection.Ascending, true) => ((IOrderedEnumerable<UsersEntity>)query).ThenBy(user => user.Name),
            (UsersSortColumn.Name, _, true) => ((IOrderedEnumerable<UsersEntity>)query).ThenByDescending(user => user.Name),
            (UsersSortColumn.CreatedDate, SortDirection.Ascending, true) => ((IOrderedEnumerable<UsersEntity>)query).ThenBy(user => user.CreatedDate),
            (UsersSortColumn.CreatedDate, _, true) => ((IOrderedEnumerable<UsersEntity>)query).ThenByDescending(user => user.CreatedDate),
            (UsersSortColumn.IsActive, SortDirection.Ascending, true) => ((IOrderedEnumerable<UsersEntity>)query).ThenBy(user => user.IsActive),
            _ => ((IOrderedEnumerable<UsersEntity>)query).ThenByDescending(user => user.IsActive)
        };
}
