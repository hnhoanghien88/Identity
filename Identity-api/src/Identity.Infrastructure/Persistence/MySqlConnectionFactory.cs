using MySqlConnector;

namespace Identity.Infrastructure.Persistence;

public sealed class MySqlConnectionFactory(string connectionString)
{
    public MySqlConnection CreateConnection() => new(connectionString);
}
