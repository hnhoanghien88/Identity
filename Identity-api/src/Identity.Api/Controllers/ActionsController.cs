using System.IdentityModel.Tokens.Jwt;
using Identity.Api.Models;
using Identity.Application.Actions.CreateAction;
using Identity.Application.Actions.DeleteAction;
using Identity.Application.Actions.Dtos;
using Identity.Application.Actions.GetActionById;
using Identity.Application.Actions.GetActions;
using Identity.Application.Actions.UpdateAction;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/actions")]
public sealed class ActionsController(ISender sender) : ControllerBase
{
    [HttpPost("search")]
    [Authorize(Policy = "Actions.Read")]
    public async Task<ActionResult<ApiResponse<PagedActionsDto>>> Search(GetActionsQuery query, CancellationToken cancellationToken)
    {
        var actions = await sender.Send(query, cancellationToken);
        return Ok(new ApiResponse<PagedActionsDto>(true, actions, "Actions retrieved successfully."));
    }

    [HttpGet("{id:long}")]

    [Authorize(Policy = "Actions.Read")]
    public async Task<ActionResult<ApiResponse<ActionDto>>> GetById(ulong id, CancellationToken cancellationToken)
    {
        var action = await sender.Send(new GetActionByIdQuery(id), cancellationToken);
        return Ok(new ApiResponse<ActionDto>(true, action, "Action retrieved successfully."));
    }

    [HttpPost]

    [Authorize(Policy = "Actions.Create")]
    public async Task<ActionResult<ApiResponse<ActionDto>>> Create(CreateRequest request, CancellationToken cancellationToken)
    {
        var action = await sender.Send(new CreateActionCommand(request.Code, request.Name, Actor()), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = action.Id }, new ApiResponse<ActionDto>(true, action, "Action created successfully."));
    }

    [HttpPut("{id:long}")]

    [Authorize(Policy = "Actions.Update")]
    public async Task<ActionResult<ApiResponse<ActionDto>>> Update(ulong id, UpdateRequest request, CancellationToken cancellationToken)
    {
        var action = await sender.Send(new UpdateActionCommand(id, request.Code, request.Name, request.Version, Actor()), cancellationToken);
        return Ok(new ApiResponse<ActionDto>(true, action, "Action updated successfully."));
    }

    [HttpDelete("{id:long}")]

    [Authorize(Policy = "Actions.Delete")]
    public async Task<IActionResult> Delete(ulong id, [FromQuery] ulong version, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteActionCommand(id, version, Actor()), cancellationToken);
        return Ok(new ApiResponse<object>(true, null, "Action deleted successfully."));
    }

    private string? Actor() =>
        User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
        ?? User.FindFirst("email")?.Value
        ?? User.FindFirst("code")?.Value;

    public sealed record CreateRequest(string Code, string Name);
    public sealed record UpdateRequest(string Code, string Name, ulong Version);
}

