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
        services.AddScoped<IApplicationsRepository, MySqlApplicationsRepository>();
        services.AddScoped<IApplicationsReadRepository, DapperApplicationsReadRepository>();
        services.AddScoped<IResourcesRepository, MySqlResourcesRepository>();
        services.AddScoped<IResourcesReadRepository, DapperResourcesReadRepository>();
        services.AddScoped<IActionsRepository, MySqlActionsRepository>();
        services.AddScoped<IActionsReadRepository, DapperActionsReadRepository>();
        services.AddScoped<IRolesRepository, MySqlRolesRepository>();
        services.AddScoped<IRolesReadRepository, DapperRolesReadRepository>();
        services.AddScoped<IRolePermissionsRepository, MySqlRolePermissionsRepository>();
        services.AddScoped<IMenusRepository, MySqlMenusRepository>();
        services.AddScoped<IMenusReadRepository, DapperMenusReadRepository>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton(new MySqlConnectionFactory(connectionString));
        services.AddScoped<IUsersReadRepository, DapperUsersReadRepository>();
        services.AddScoped<IUserRolesReadRepository, DapperUserRolesReadRepository>();
        services.AddScoped<IRefreshTokenRepository, MySqlRefreshTokenRepository>();
        return services;
    }
}
