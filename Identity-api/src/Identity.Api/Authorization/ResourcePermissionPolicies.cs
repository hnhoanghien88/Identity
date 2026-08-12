using Identity.Application.Common.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Authorization;

public static class ResourcePermissionPolicies
{
    public static AuthorizationOptions AddResourcePolicies(this AuthorizationOptions options)
    {
        foreach (var permission in ResourcePermissions.All)
        {
            options.AddPolicy(
                permission,
                policy => policy
                    .RequireAuthenticatedUser()
                    .RequireClaim("permission", permission));
        }
        return options;
    }
}
