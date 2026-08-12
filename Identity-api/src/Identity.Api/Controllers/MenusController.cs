using System.IdentityModel.Tokens.Jwt;
using Identity.Api.Models;
using Identity.Application.Common.Authorization;
using Identity.Application.Menus.CreateMenu;
using Identity.Application.Menus.DeleteMenu;
using Identity.Application.Menus.Dtos;
using Identity.Application.Menus.GetMenuById;
using Identity.Application.Menus.GetMenus;
using Identity.Application.Menus.UpdateMenu;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/menus")]
public sealed class MenusController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MenuDto>>>> Get(ulong applicationId, CancellationToken token)
    {
        var result = await sender.Send(new GetMenusQuery(applicationId), token);
        return Ok(new ApiResponse<IReadOnlyList<MenuDto>>(true, result, "Menus retrieved successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<MenuDto>>> GetById(ulong id, CancellationToken token)
    {
        var result = await sender.Send(new GetMenuByIdQuery(id), token);
        return Ok(new ApiResponse<MenuDto>(true, result, "Menu retrieved successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MenuDto>>> Create(CreateRequest request, CancellationToken token)
    {
        var result = await sender.Send(new CreateMenuCommand(request.ApplicationId, request.ParentId,
            request.ResourceId, request.Code, request.Name, request.Route, request.Icon, request.SortOrder,
            request.IsVisible, request.IsActive, Actor()), token);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new ApiResponse<MenuDto>(true, result, "Menu created successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<MenuDto>>> Update(ulong id, UpdateRequest request, CancellationToken token)
    {
        var result = await sender.Send(new UpdateMenuCommand(id, request.ApplicationId, request.ParentId,
            request.ResourceId, request.Code, request.Name, request.Route, request.Icon, request.SortOrder,
            request.IsVisible, request.IsActive, request.Version, Actor()), token);
        return Ok(new ApiResponse<MenuDto>(true, result, "Menu updated successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(ulong id, [FromQuery] ulong version, CancellationToken token)
    {
        await sender.Send(new DeleteMenuCommand(id, version, Actor()), token);
        return Ok(new ApiResponse<object>(true, null, "Menu deleted successfully."));
    }

    private string? Actor() => User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
        ?? User.FindFirst("email")?.Value ?? User.FindFirst("code")?.Value;

    public record CreateRequest(ulong ApplicationId, ulong? ParentId, ulong? ResourceId, string Code,
        string Name, string? Route, string? Icon, int SortOrder, bool IsVisible = true, bool IsActive = true);
    public sealed record UpdateRequest(ulong ApplicationId, ulong? ParentId, ulong? ResourceId, string Code,
        string Name, string? Route, string? Icon, int SortOrder, bool IsVisible, bool IsActive, ulong Version)
        : CreateRequest(ApplicationId, ParentId, ResourceId, Code, Name, Route, Icon, SortOrder, IsVisible, IsActive);
}

