using System.IdentityModel.Tokens.Jwt;
using Identity.Api.Models;
using Identity.Application.Applications.CreateApplication;
using Identity.Application.Applications.DeleteApplication;
using Identity.Application.Applications.Dtos;
using Identity.Application.Applications.GetApplicationById;
using Identity.Application.Applications.GetApplications;
using Identity.Application.Applications.UpdateApplication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/applications")]
public sealed class ApplicationsController(ISender sender) : ControllerBase
{
    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse<PagedApplicationsDto>>> Search(
        GetApplicationsQuery query,
        CancellationToken cancellationToken)
    {
        var applications = await sender.Send(query, cancellationToken);
        return Ok(new ApiResponse<PagedApplicationsDto>(true, applications, "Applications retrieved successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> GetById(
        ulong id,
        CancellationToken cancellationToken)
    {
        var application = await sender.Send(new GetApplicationByIdQuery(id), cancellationToken);
        return Ok(new ApiResponse<ApplicationDto>(true, application, "Application retrieved successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> Create(
        CreateRequest request,
        CancellationToken cancellationToken)
    {
        var application = await sender.Send(
            new CreateApplicationCommand(
                request.Code,
                request.Name,
                request.Audience,
                request.Description,
                Actor()),
            cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { id = application.Id },
            new ApiResponse<ApplicationDto>(true, application, "Application created successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> Update(
        ulong id,
        UpdateRequest request,
        CancellationToken cancellationToken)
    {
        var application = await sender.Send(
            new UpdateApplicationCommand(
                id,
                request.Code,
                request.Name,
                request.Audience,
                request.Description,
                request.IsActive,
                request.Version,
                Actor()),
            cancellationToken);
        return Ok(new ApiResponse<ApplicationDto>(true, application, "Application updated successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        ulong id,
        [FromQuery] ulong version,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteApplicationCommand(id, version, Actor()), cancellationToken);
        return Ok(new ApiResponse<object>(true, null, "Application deleted successfully."));
    }

    private string? Actor() =>
        User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
        ?? User.FindFirst("email")?.Value
        ?? User.FindFirst("code")?.Value;

    public sealed record CreateRequest(string Code, string Name, string Audience, string? Description);
    public sealed record UpdateRequest(string Code, string Name, string Audience, string? Description, bool IsActive, ulong Version);
}
