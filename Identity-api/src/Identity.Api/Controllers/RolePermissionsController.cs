using System.IdentityModel.Tokens.Jwt;
using Identity.Api.Models;
using Identity.Application.RolePermissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/role-permissions")]
public sealed class RolePermissionsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<RolePermissionSnapshotDto>>> Get(
        [FromQuery] ulong roleId,
        [FromQuery] ulong resourceId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetRolePermissionSnapshotQuery(roleId, resourceId),
            cancellationToken);
        return Ok(new ApiResponse<RolePermissionSnapshotDto>(
            true,
            result,
            "Role Permissions retrieved successfully."));
    }

    [HttpPut("{roleId:long}/{resourceId:long}/{actionId:long}")]
    public async Task<IActionResult> Grant(
        ulong roleId,
        ulong resourceId,
        ulong actionId,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new GrantRolePermissionCommand(
                roleId,
                resourceId,
                actionId,
                Actor()),
            cancellationToken);
        return Ok(new ApiResponse<object>(
            true,
            null,
            "Role Permission granted successfully."));
    }

    [HttpDelete("{roleId:long}/{resourceId:long}/{actionId:long}")]
    public async Task<IActionResult> Revoke(
        ulong roleId,
        ulong resourceId,
        ulong actionId,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new RevokeRolePermissionCommand(
                roleId,
                resourceId,
                actionId,
                Actor()),
            cancellationToken);
        return Ok(new ApiResponse<object>(
            true,
            null,
            "Role Permission revoked successfully."));
    }

    private string? Actor() =>
        User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
        ?? User.FindFirst("email")?.Value
        ?? User.FindFirst("code")?.Value;
}
