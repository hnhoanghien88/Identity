using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Authorization;

public sealed record PermissionRequirement(string Permission)
    : IAuthorizationRequirement;

public sealed class PermissionAuthorizationHandler(
    IAuthorizationCache authorizationCache)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdValue = context.User.FindFirstValue("uid");
        var versionValue = context.User.FindFirstValue("permissionversion");
        if (!ulong.TryParse(userIdValue, out var userId)
            || !int.TryParse(versionValue, out var permissionVersion))
            return;

        var authorization = await authorizationCache.GetAsync(
            userId,
            permissionVersion,
            CancellationToken.None);
        if (authorization.Permissions.Contains(
                requirement.Permission,
                StringComparer.Ordinal))
            context.Succeed(requirement);
    }
}