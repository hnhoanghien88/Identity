using System.Text;
using Dapper;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Actions.Dtos;
using Identity.Application.Actions.GetActions;
using Identity.Application.Users.GetUsers;

namespace Identity.Infrastructure.Persistence;

public sealed class DapperActionsReadRepository(MySqlConnectionFactory connectionFactory) : IActionsReadRepository
{
    private const string SelectActions = "SELECT Id, Code, Name, CreatedDate, Version FROM permission_actions";

    public async Task<ActionDto?> GetByIdAsync(ulong id, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ActionDto>(
            new CommandDefinition($"{SelectActions} WHERE Id = @Id", new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<PagedActionsDto> GetAsync(ActionsFilter filter, IReadOnlyList<ActionsSort> sorts, int page, int pageSize, CancellationToken cancellationToken)
    {
        var where = new StringBuilder(" WHERE 1 = 1");
        var parameters = new DynamicParameters();
        AddStringFilter(where, parameters, "Code", filter.Code);
        AddStringFilter(where, parameters, "Name", filter.Name);
        var sql = new StringBuilder(SelectActions).Append(where);
        sql.Append(" ORDER BY ").Append(BuildOrderBy(sorts));
        sql.Append(" LIMIT @PageSize OFFSET @Offset");
        parameters.Add("PageSize", pageSize);
        parameters.Add("Offset", (page - 1) * pageSize);
        await using var connection = connectionFactory.CreateConnection();
        var actions = await connection.QueryAsync<ActionDto>(
            new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken));
        var totalCount = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition($"SELECT COUNT(*) FROM permission_actions{where}", parameters, cancellationToken: cancellationToken));
        return new PagedActionsDto(actions.AsList(), totalCount, page, pageSize);
    }

    private static void AddStringFilter(StringBuilder sql, DynamicParameters parameters, string column, StringFilter? filter)
    {
        if (filter?.Values is { Count: > 0 })
        {
            sql.Append($" AND {column} IN @{column}Values");
            parameters.Add($"{column}Values", filter.Values);
        }
        AddLike(sql, parameters, column, "Contains", filter?.Contains, value => $"%{value}%");
        AddLike(sql, parameters, column, "StartsWith", filter?.StartsWith, value => $"{value}%");
        AddLike(sql, parameters, column, "EndsWith", filter?.EndsWith, value => $"%{value}");
    }

    private static void AddLike(StringBuilder sql, DynamicParameters parameters, string column, string suffix, string? value, Func<string, string> pattern)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        var parameterName = $"{column}{suffix}";
        sql.Append($" AND {column} LIKE @{parameterName}");
        parameters.Add(parameterName, pattern(value.Trim()));
    }

    private static string BuildOrderBy(IReadOnlyList<ActionsSort> sorts) =>
        string.Join(", ", sorts.Select(sort =>
        {
            var column = sort.Column switch
            {
                ActionsSortColumn.Id => "Id",
                ActionsSortColumn.Code => "Code",
                ActionsSortColumn.Name => "Name",
                ActionsSortColumn.CreatedDate => "CreatedDate",
                _ => throw new ArgumentOutOfRangeException(nameof(sort.Column)),
            };
            var direction = sort.Direction == SortDirection.Descending ? "DESC" : "ASC";
            return $"{column} {direction}";
        }));
}

