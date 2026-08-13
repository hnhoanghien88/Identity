using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Authorization;

public static class PermissionPolicies
{
    private static readonly string[] Resources =
    [
        "Users", "Roles", "RolePermissions", "Resources",
        "Applications", "Menus", "Actions", "UserRoles"
    ];

    private static readonly string[] Actions =
    [
        "Read", "Create", "Update", "Delete", "Export", "Import"
    ];

    public static AuthorizationOptions AddPermissionPolicies(this AuthorizationOptions options)
    {
        foreach (var resource in Resources)
        {
            foreach (var action in Actions)
            {
                var permission = $"{resource}.{action}";
                options.AddPolicy(
                    permission,
                    policy => policy
                        .RequireAuthenticatedUser()
                        .AddRequirements(new PermissionRequirement(permission)));
            }
        }

        return options;
    }
}