using System.IdentityModel.Tokens.Jwt;
using Identity.Api.Models;
using Identity.Application.Resources.CreateResource;
using Identity.Application.Resources.DeleteResource;
using Identity.Application.Resources.Dtos;
using Identity.Application.Resources.GetResourceById;
using Identity.Application.Resources.GetResources;
using Identity.Application.Resources.UpdateResource;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/resources")]
public sealed class ResourcesController(ISender sender) : ControllerBase
{
    [HttpPost("search")]
    [Authorize(Policy = "Resources.Read")]
    public async Task<ActionResult<ApiResponse<PagedResourcesDto>>> Search(
        GetResourcesQuery query,
        CancellationToken cancellationToken)
    {
        var resources = await sender.Send(query, cancellationToken);
        return Ok(
            new ApiResponse<PagedResourcesDto>(
                true,
                resources,
                "Resources retrieved successfully."));
    }

    [HttpGet("{id:long}")]

    [Authorize(Policy = "Resources.Read")]
    public async Task<ActionResult<ApiResponse<ResourceDto>>> GetById(
        ulong id,
        CancellationToken cancellationToken)
    {
        var resource = await sender.Send(
            new GetResourceByIdQuery(id),
            cancellationToken);
        return Ok(
            new ApiResponse<ResourceDto>(
                true,
                resource,
                "Resource retrieved successfully."));
    }

    [HttpPost]

    [Authorize(Policy = "Resources.Create")]
    public async Task<ActionResult<ApiResponse<ResourceDto>>> Create(
        CreateRequest request,
        CancellationToken cancellationToken)
    {
        var resource = await sender.Send(
            new CreateResourceCommand(
                request.ApplicationId,
                request.Code,
                request.Name,
                request.ResourceType,
                request.Description,
                Actor()),
            cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { id = resource.Id },
            new ApiResponse<ResourceDto>(
                true,
                resource,
                "Resource created successfully."));
    }

    [HttpPut("{id:long}")]

    [Authorize(Policy = "Resources.Update")]
    public async Task<ActionResult<ApiResponse<ResourceDto>>> Update(
        ulong id,
        UpdateRequest request,
        CancellationToken cancellationToken)
    {
        var resource = await sender.Send(
            new UpdateResourceCommand(
                id,
                request.ApplicationId,
                request.Code,
                request.Name,
                request.ResourceType,
                request.Description,
                request.IsActive,
                request.Version,
                Actor()),
            cancellationToken);
        return Ok(
            new ApiResponse<ResourceDto>(
                true,
                resource,
                "Resource updated successfully."));
    }

    [HttpDelete("{id:long}")]

    [Authorize(Policy = "Resources.Delete")]
    public async Task<IActionResult> Delete(
        ulong id,
        [FromQuery] ulong version,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new DeleteResourceCommand(id, version, Actor()),
            cancellationToken);
        return Ok(
            new ApiResponse<object>(
                true,
                null,
                "Resource deleted successfully."));
    }

    private string? Actor() =>
        User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
        ?? User.FindFirst("email")?.Value
        ?? User.FindFirst("code")?.Value;

    public sealed record CreateRequest(
        ulong ApplicationId,
        string Code,
        string Name,
        string ResourceType,
        string? Description);

    public sealed record UpdateRequest(
        ulong ApplicationId,
        string Code,
        string Name,
        string ResourceType,
        string? Description,
        bool IsActive,
        ulong Version);
}
