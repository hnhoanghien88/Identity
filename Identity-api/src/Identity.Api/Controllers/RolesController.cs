using System.IdentityModel.Tokens.Jwt;
using Identity.Api.Models;
using Identity.Application.Roles.CreateRole;
using Identity.Application.Roles.DeleteRole;
using Identity.Application.Roles.Dtos;
using Identity.Application.Roles.GetRoles;
using Identity.Application.Roles.UpdateRole;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/roles")]
public sealed class RolesController(ISender sender) : ControllerBase
{
    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse<PagedRolesDto>>> Search(
        GetRolesQuery query,
        CancellationToken cancellationToken)
    {
        var roles = await sender.Send(query, cancellationToken);
        return Ok(new ApiResponse<PagedRolesDto>(true, roles, "Roles retrieved successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> GetById(ulong id, CancellationToken cancellationToken)
    {
        var role = await sender.Send(new GetRoleByIdQuery(id), cancellationToken);
        return Ok(new ApiResponse<RoleDto>(true, role, "Role retrieved successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<RoleDto>>> Create(
        CreateRequest request,
        CancellationToken cancellationToken)
    {
        var role = await sender.Send(
            new CreateRoleCommand(
                request.ApplicationId,
                request.Code,
                request.Name,
                request.IsSystemRole,
                request.IsActive,
                Actor()),
            cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { id = role.Id },
            new ApiResponse<RoleDto>(true, role, "Role created successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> Update(
        ulong id,
        UpdateRequest request,
        CancellationToken cancellationToken)
    {
        var role = await sender.Send(
            new UpdateRoleCommand(
                id,
                request.ApplicationId,
                request.Code,
                request.Name,
                request.IsSystemRole,
                request.IsActive,
                request.Version,
                Actor()),
            cancellationToken);
        return Ok(new ApiResponse<RoleDto>(true, role, "Role updated successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(ulong id, [FromQuery] ulong version, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteRoleCommand(id, version), cancellationToken);
        return Ok(new ApiResponse<object>(true, null, "Role deleted successfully."));
    }

    private string? Actor() =>
        User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
        ?? User.FindFirst("email")?.Value
        ?? User.FindFirst("code")?.Value;

    public sealed record CreateRequest(
        ulong ApplicationId,
        string Code,
        string Name,
        bool IsSystemRole,
        bool IsActive);

    public sealed record UpdateRequest(
        ulong ApplicationId,
        string Code,
        string Name,
        bool IsSystemRole,
        bool IsActive,
        ulong Version);
}
