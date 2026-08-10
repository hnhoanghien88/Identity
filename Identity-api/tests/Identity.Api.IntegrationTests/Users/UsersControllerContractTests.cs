using System.Reflection;
using Identity.Api.Controllers;
using Identity.Application.Common.Authorization;
using Identity.Application.Users.CreateUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.IntegrationTests.Users;

public sealed class UsersControllerContractTests
{
    [Fact]
    public void Create_is_not_anonymous_and_requires_management_role()
    {
        var method = typeof(UsersController).GetMethod(nameof(UsersController.Create))!;
        Assert.Empty(method.GetCustomAttributes<AllowAnonymousAttribute>());
        Assert.Contains(method.GetCustomAttributes<AuthorizeAttribute>(), x => x.Roles == RoleGroups.Management);
    }

    [Fact]
    public void Delete_requires_admin_and_version_query()
    {
        var method = typeof(UsersController).GetMethod(nameof(UsersController.Delete))!;
        Assert.Contains(method.GetCustomAttributes<AuthorizeAttribute>(), x => x.Roles == RoleNames.Admin);
        Assert.Contains(method.GetParameters(), x => x.Name == "version" && x.GetCustomAttribute<FromQueryAttribute>() is not null);
    }

    [Fact]
    public void Create_and_update_contracts_carry_code_and_email_separately()
    {
        var createProperties = typeof(CreateUsersCommand)
            .GetProperties()
            .Select(x => x.Name)
            .ToHashSet();
        var updateProperties = typeof(UsersController.UpdateRequest)
            .GetProperties()
            .Select(x => x.Name)
            .ToHashSet();

        Assert.Contains("Code", createProperties);
        Assert.Contains("Email", createProperties);
        Assert.Contains("Code", updateProperties);
        Assert.Contains("Email", updateProperties);
    }
}
