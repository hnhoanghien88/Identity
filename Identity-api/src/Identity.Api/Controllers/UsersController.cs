using Identity.Application.Users.ActivateUsers;
using Identity.Application.Users.CreateUsers;
using Identity.Application.Users.DeleteUsers;
using Identity.Application.Users.GetUsers;
using Identity.Application.Users.GetUsersById;
using Identity.Application.Users.UpdateUsers;
using Identity.Application.Users.Dtos;
using Identity.Api.Models;
using Identity.Application.Common.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = RoleGroups.All)]
public sealed class UsersController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = RoleGroups.Management)]
    public async Task<IActionResult> Create(CreateUsersCommand command, CancellationToken ct)
    {
        var user = await sender.Send(command, ct);
        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            new ApiResponse<UsersDto>(true, user, "User created successfully."));
    }

    [HttpGet("{id:long}")]
    [Authorize(Roles = RoleGroups.Management)]
    public async Task<ActionResult<ApiResponse<UsersDto>>> GetById(ulong id, CancellationToken ct)
    {
        var user = await sender.Send(new GetUsersByIdQuery(id), ct);
        return Ok(new ApiResponse<UsersDto>(true, user, "User retrieved successfully."));
    }

    /// <summary>
    /// Search users.
    /// </summary>
    /// <remarks>
    /// Request without filters:
    ///
    ///     {
    ///       "filter": null,
    ///       "sorts": null,
    ///       "page": 1,
    ///       "pageSize": 20
    ///     }
    ///
    /// Column: 0 = Id, 1 = Code, 2 = Name, 3 = CreatedDate, 4 = IsActive.
    ///
    /// Direction: 0 = ascending, 1 = descending.
    ///
    /// Page and pageSize must be greater than or equal to 1. 
    ///     
    ///     {
    ///         "filter": {
    ///             "code": {
    ///                 "contains": "USR"
    ///             },
    ///             "name": {
    ///                 "contains": "Nguyen"
    ///             },
    ///             "isActive": true
    ///             },
    ///             "sorts": [
    ///                 {
    ///                     "column": 3,
    ///                     "direction": 1
    ///                 }
    ///             ],
    ///         "page": 1,
    ///         "pageSize": 20
    ///     }
    ///
    /// </remarks>
    [HttpPost("search")]
    [Authorize(Roles = RoleGroups.Management)]
    public async Task<ActionResult<ApiResponse<PagedUsersDto>>> Get(GetUsersQuery query, CancellationToken ct)
    {
        var users = await sender.Send(query, ct);
        return Ok(new ApiResponse<PagedUsersDto>(true, users, "Users retrieved successfully."));
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = RoleGroups.Management)]
    public async Task<ActionResult<ApiResponse<UsersDto>>> Update(ulong id, UpdateRequest request, CancellationToken ct)
    {
        var user = await sender.Send(
            new UpdateUsersCommand(
                id,
                request.Code,
                request.Email,
                request.Name,
                request.Version),
            ct);
        return Ok(new ApiResponse<UsersDto>(true, user, "User updated successfully."));
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(ulong id, [FromQuery] ulong version, CancellationToken ct)
    {
        var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!ulong.TryParse(subject, out var actorId))
            throw new UnauthorizedAccessException("The access token subject is invalid.");
        await sender.Send(new DeleteUsersCommand(id, actorId, version, User.FindFirst("email")?.Value), ct);
        return Ok(new ApiResponse<object>(true, null, "User deleted successfully."));
    }

    [HttpPatch("{id:long}/activation")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Activate(ulong id, ActivationRequest request, CancellationToken ct)
    {
        await sender.Send(new ActivateUsersCommand(id, request.IsActive), ct);
        return Ok(new ApiResponse<object>(true, null, "User activation status updated successfully."));
    }

    public sealed record UpdateRequest(
        string Code,
        string Email,
        string Name,
        ulong Version);

    public sealed record ActivationRequest(bool IsActive);
}
