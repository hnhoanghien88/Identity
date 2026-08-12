using System.Diagnostics;
using System.Text;
using System.Text.Json;
using MySqlConnector;

namespace Identity.Infrastructure.IntegrationTests.Resources;

public sealed class ResourcePerformanceTests
{
    [Fact]
    public async Task Search_page_over_ten_thousand_resources_completes_under_two_seconds()
    {
        var settingsPath = Path.GetFullPath(
            "../../../../../src/Identity.Api/appsettings.Development.json",
            AppContext.BaseDirectory);
        using var settings = JsonDocument.Parse(await File.ReadAllTextAsync(settingsPath));
        var connectionString = settings.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("IdentityDatabase")
            .GetString();

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        var applicationCode = $"PERF_{Guid.NewGuid():N}";
        var insertApplication = new MySqlCommand(
            """
            INSERT INTO applications
                (Code, Name, Audience, Description, Version, CreatedDate, IsActive, IsDeleted)
            VALUES
                (@Code, 'Resource performance fixture', @Code, NULL, 1, UTC_TIMESTAMP(6), 1, 0);
            SELECT LAST_INSERT_ID();
            """,
            connection,
            transaction);
        insertApplication.Parameters.AddWithValue("Code", applicationCode);
        var applicationId = Convert.ToUInt64(
            await insertApplication.ExecuteScalarAsync());

        for (var batchStart = 0; batchStart < 10_000; batchStart += 500)
        {
            var sql = new StringBuilder(
                """
                INSERT INTO resources
                    (ApplicationId, Code, Name, ResourceType, Description,
                     Version, CreatedDate, IsActive, IsDeleted)
                VALUES
                """);
            var command = new MySqlCommand
            {
                Connection = connection,
                Transaction = transaction,
            };
            for (var offset = 0; offset < 500; offset++)
            {
                var index = batchStart + offset;
                if (offset > 0)
                    sql.Append(',');
                sql.Append(
                    $"(@ApplicationId, @Code{index}, @Name{index}, 'Api', NULL, 1, UTC_TIMESTAMP(6), 1, 0)");
                command.Parameters.AddWithValue($"Code{index}", $"RESOURCE_{index:D5}");
                command.Parameters.AddWithValue($"Name{index}", $"Resource {index:D5}");
            }
            command.Parameters.AddWithValue("ApplicationId", applicationId);
            command.CommandText = sql.ToString();
            await command.ExecuteNonQueryAsync();
        }

        var search = new MySqlCommand(
            """
            SELECT r.Id, r.Code, r.Name, r.ResourceType
            FROM resources r
            WHERE r.IsDeleted = 0
              AND r.ApplicationId = @ApplicationId
              AND r.Code LIKE '%RESOURCE%'
            ORDER BY r.CreatedDate DESC, r.Id DESC
            LIMIT 20 OFFSET 0
            """,
            connection,
            transaction);
        search.Parameters.AddWithValue("ApplicationId", applicationId);

        var stopwatch = Stopwatch.StartNew();
        var count = 0;
        await using (var reader = await search.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
                count++;
        }
        stopwatch.Stop();

        Assert.Equal(20, count);
        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(2),
            $"Search took {stopwatch.Elapsed.TotalMilliseconds:F0} ms.");

        await transaction.RollbackAsync();
    }
}
