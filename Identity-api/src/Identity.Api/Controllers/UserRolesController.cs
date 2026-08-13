using System.IdentityModel.Tokens.Jwt;
using Identity.Api.Models;
using Identity.Application.UserRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Identity.Api.Controllers;

[Authorize, ApiController, Route("api/user-roles")]
public sealed class UserRolesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedUserRoleUsersDto>>> Get(ulong roleId, int page = 1, int pageSize = 20, CancellationToken ct = default) => Ok(new ApiResponse<PagedUserRoleUsersDto>(true, await sender.Send(new GetRoleMembersQuery(roleId, page, pageSize), ct), "Role members retrieved successfully."));
    [HttpGet("candidates")]
    public async Task<ActionResult<ApiResponse<PagedUserRoleUsersDto>>> Candidates(ulong roleId, string? search = null, int page = 1, int pageSize = 20, CancellationToken ct = default) => Ok(new ApiResponse<PagedUserRoleUsersDto>(true, await sender.Send(new GetRoleCandidatesQuery(roleId, search, page, pageSize), ct), "Role candidates retrieved successfully."));
    [HttpPost("{roleId:long}")]
    public async Task<ActionResult<ApiResponse<AssignUsersToRoleResultDto>>> Assign(ulong roleId, AssignRequest request, CancellationToken ct) => Ok(new ApiResponse<AssignUsersToRoleResultDto>(true, await sender.Send(new AssignUsersToRoleCommand(roleId, request.UserIds, Actor()), ct), "Users assigned successfully."));
    [HttpDelete("{roleId:long}/{userId:long}")]
    public async Task<IActionResult> Remove(ulong roleId, ulong userId, CancellationToken ct) { await sender.Send(new RemoveUserFromRoleCommand(roleId, userId), ct); return Ok(new ApiResponse<object>(true, null, "User removed from Role successfully.")); }
    private string? Actor() => User.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? User.FindFirst("email")?.Value ?? User.FindFirst("code")?.Value;
    public sealed record AssignRequest(IReadOnlyCollection<ulong> UserIds);
}
