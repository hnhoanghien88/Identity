using Identity.Application.Abstractions.Persistence;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentityDatabase")
            ?? throw new InvalidOperationException("Connection string 'IdentityDatabase' was not found.");
        services.AddDbContext<IdentityDbContext>(options => options.UseMySQL(connectionString));
        services.AddScoped<IUsersRepository, MySqlUsersRepository>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton(new MySqlConnectionFactory(connectionString));
        services.AddScoped<IUsersReadRepository, DapperUsersReadRepository>();
        return services;
    }
}
