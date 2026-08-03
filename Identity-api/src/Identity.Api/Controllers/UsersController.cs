using Identity.Application.Users.ActivateUsers;
using Identity.Application.Users.AuthenticateUser;
using Identity.Application.Users.CreateUsers;
using Identity.Application.Users.DeleteUsers;
using Identity.Application.Users.GetUsers;
using Identity.Application.Users.GetUsersById;
using Identity.Application.Users.UpdateUsers;
using Identity.Application.Users.Dtos;
using Identity.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateUsersCommand command, CancellationToken ct)
    {
        var user = await sender.Send(command, ct);
        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            new ApiResponse<UsersDto>(true, user, "User created successfully."));
    }

    [HttpPost("authenticate")]
    public async Task<ActionResult<ApiResponse<UsersDto>>> Authenticate(AuthenticateUserQuery query, CancellationToken ct)
    {
        var user = await sender.Send(query, ct);
        return Ok(new ApiResponse<UsersDto>(true, user, "Credentials are valid."));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UsersDto>>> GetById(Guid id, CancellationToken ct)
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
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UsersDto>>>> Get(GetUsersQuery query, CancellationToken ct)
    {
        var users = await sender.Send(query, ct);
        return Ok(new ApiResponse<IReadOnlyList<UsersDto>>(true, users, "Users retrieved successfully."));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UsersDto>>> Update(Guid id, UpdateRequest request, CancellationToken ct)
    {
        var user = await sender.Send(new UpdateUsersCommand(id, request.Code, request.Name), ct);
        return Ok(new ApiResponse<UsersDto>(true, user, "User updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteUsersCommand(id), ct);
        return Ok(new ApiResponse<object>(true, null, "User deleted successfully."));
    }

    [HttpPatch("{id:guid}/activation")]
    public async Task<IActionResult> Activate(Guid id, ActivationRequest request, CancellationToken ct)
    {
        await sender.Send(new ActivateUsersCommand(id, request.IsActive), ct);
        return Ok(new ApiResponse<object>(true, null, "User activation status updated successfully."));
    }

    public sealed record UpdateRequest(string Code, string Name);

    public sealed record ActivationRequest(bool IsActive);
}



