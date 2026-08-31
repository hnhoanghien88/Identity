using System.Security.Claims;
using Identity.Api.Authorization;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Applications.GetApplications;
using Identity.Application.Menus.Dtos;
using Identity.Application.Menus.GetMenus;
using Identity.Application.Users.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("authorization")]
public sealed class AuthorizationController(
    IAuthorizationCache authorizationCache,
    IApplicationsReadRepository applications,
    ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AuthorizationResponse>> Get(
        [FromQuery] string? applicationCode,
        CancellationToken cancellationToken)
    {
        if (!ulong.TryParse(User.FindFirstValue("sub"), out var userId)
            || !int.TryParse(
                User.FindFirstValue("permissionversion"),
                out var permissionVersion))
            throw new UnauthorizedAccessException("Authorization claims are invalid.");

        if (string.IsNullOrWhiteSpace(applicationCode))
        {
            ModelState.AddModelError(
                nameof(applicationCode),
                "Application code is required.");
            return ValidationProblem(ModelState);
        }

        var requestedApplicationCode = applicationCode.Trim();
        if (!string.Equals(
                User.FindFirstValue("application_code"),
                requestedApplicationCode,
                StringComparison.Ordinal))
            return Forbid();

        var application = await applications.GetAsync(
            new ApplicationsFilter(
                Code: new StringFilter(Values: [requestedApplicationCode]),
                IsActive: true),
            [new ApplicationsSort(ApplicationsSortColumn.Code, SortDirection.Ascending)],
            1,
            1,
            cancellationToken);
        if (application.Items.Count == 0)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Application not found",
                Detail = $"Application '{requestedApplicationCode}' does not exist or is inactive."
            });
        }

        var authorization = await authorizationCache.GetAsync(
            userId,
            permissionVersion,
            requestedApplicationCode,
            cancellationToken);
        var menus = await sender.Send(
            new GetMenusQuery(application.Items[0].Id),
            cancellationToken);
        var visibleMenus = FilterMenus(
            menus,
            authorization.Permissions.ToHashSet(StringComparer.Ordinal))
            .Select(ToRuntimeMenu)
            .ToArray();

        return Ok(new AuthorizationResponse(
            authorization.Roles,
            authorization.Permissions,
            visibleMenus));
    }

    private static RuntimeMenuResponse ToRuntimeMenu(MenuDto menu) =>
        new(
            menu.Id,
            menu.Name,
            menu.Route,
            menu.Icon,
            menu.Children.Select(ToRuntimeMenu).ToArray());

    private static IReadOnlyList<MenuDto> FilterMenus(
        IReadOnlyList<MenuDto> menus,
        IReadOnlySet<string> permissions)
    {
        var result = new List<MenuDto>();

        foreach (var menu in menus.Where(menu => menu.IsActive))
        {
            var children = FilterMenus(menu.Children, permissions);

            if (menu.IsVisible)
            {
                result.AddRange(children);
                continue;
            }

            if (children.Count > 0
                || (menu.ResourceCode is not null
                    && permissions.Contains($"{menu.ResourceCode}.ViewMenu")))
                result.Add(menu with { Children = children });
        }

        return result;
    }

    public sealed record AuthorizationResponse(
        IReadOnlyList<string> Roles,
        IReadOnlyList<string> Permissions,
        IReadOnlyList<RuntimeMenuResponse> Menus);

    public sealed record RuntimeMenuResponse(
        ulong Id,
        string Name,
        string? Route,
        string? Icon,
        IReadOnlyList<RuntimeMenuResponse> Children);
}
