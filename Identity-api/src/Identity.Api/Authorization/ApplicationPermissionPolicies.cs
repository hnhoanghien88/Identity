using Identity.Application.Common.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Authorization;

public static class ApplicationPermissionPolicies
{
    public static AuthorizationOptions AddApplicationPolicies(this AuthorizationOptions options)
    {
        foreach (var permission in ApplicationPermissions.All)
        {
            options.AddPolicy(
                permission,
                policy => policy.RequireAuthenticatedUser());
        }
        return options;
    }
}
