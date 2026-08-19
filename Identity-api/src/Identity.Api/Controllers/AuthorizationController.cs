using System.Security.Claims;
using Identity.Api.Authentication;
using Identity.Api.Authorization;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Applications.GetApplications;
using Identity.Application.Menus.Dtos;
using Identity.Application.Menus.GetMenus;
using Identity.Application.Users.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("authorization")]
public sealed class AuthorizationController(
    IAuthorizationCache authorizationCache,
    IApplicationsReadRepository applications,
    ISender sender,
    IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AuthorizationResponse>> Get(
        CancellationToken cancellationToken)
    {
        if (!ulong.TryParse(User.FindFirstValue("uid"), out var userId)
            || !int.TryParse(
                User.FindFirstValue("permissionversion"),
                out var permissionVersion))
            throw new UnauthorizedAccessException("Authorization claims are invalid.");

        var authorization = await authorizationCache.GetAsync(
            userId,
            permissionVersion,
            cancellationToken);
        var application = await applications.GetAsync(
            new ApplicationsFilter(
                Code: new StringFilter(Values: [jwtOptions.Value.ApplicationCode]),
                IsActive: true),
            [new ApplicationsSort(ApplicationsSortColumn.Code, SortDirection.Ascending)],
            1,
            1,
            cancellationToken);
        var menus = application.Items.Count == 0
            ? []
            : await sender.Send(
                new GetMenusQuery(application.Items[0].Id),
                cancellationToken);
        var visibleMenus = FilterMenus(
            menus,
            authorization.Permissions.ToHashSet(StringComparer.Ordinal));

        return Ok(new AuthorizationResponse(
            authorization.Roles,
            authorization.Permissions,
            visibleMenus));
    }

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
        IReadOnlyList<MenuDto> Menus);
}