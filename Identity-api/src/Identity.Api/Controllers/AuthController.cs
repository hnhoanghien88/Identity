using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Identity.Api.Authentication;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Applications.Dtos;
using Identity.Application.Applications.GetApplications;
using Identity.Application.Common.Exceptions;
using Identity.Application.Users.AuthenticateUser;
using Identity.Application.Users.GetUsersById;
using Identity.Application.Users.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Identity.Api.Controllers;

[ApiController]
public sealed class AuthController(
    ISender sender,
    IJwtTokenService tokens,
    IRefreshTokenRepository refreshTokens,
    IUserRolesReadRepository userRoles,
    IApplicationsReadRepository applications,
    IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    [AllowAnonymous]
    [HttpPost("/login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken ct)
    {
        var user = await sender.Send(
            new AuthenticateUserQuery(request.Code, request.Password), ct);
        var application = await GetActiveApplicationAsync(
            request.ApplicationCode ?? _jwtOptions.ApplicationCode,
            ct);
        var authorization = await userRoles.GetAuthorizationAsync(user.Id, application.Code, ct);
        if (authorization.Roles.Count == 0)
            throw new UnauthorizedAccessException("The user has no active application role.");
        var accessToken = tokens.CreateAccessToken(user, authorization, application);
        var refreshToken = await refreshTokens.IssueForLoginAsync(
            user.Id,
            user.Email,
            application.Code,
            TimeSpan.FromDays(_jwtOptions.RefreshTokenDays),
            ct);

        Authentication.RefreshTokenCookie.Write(
            HttpContext,
            application.Code,
            refreshToken.Token,
            refreshToken.ExpiresAtUtc);
        return Ok(ToResponse(accessToken));
    }

    [AllowAnonymous]
    [HttpPost("/refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(
        [FromQuery] string? applicationCode,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(applicationCode))
            return MissingApplicationCode();

        var requestedApplicationCode = applicationCode.Trim();
        if (!Authentication.RefreshTokenCookie.TryRead(
                HttpContext,
                requestedApplicationCode,
                out var currentToken))
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "No refresh session is available."
            });
        }

        var rotatedToken = await refreshTokens.RotateAsync(
            currentToken,
            TimeSpan.FromDays(_jwtOptions.RefreshTokenDays),
            ct);

        var user = await sender.Send(
            new GetUsersByIdQuery(rotatedToken.UserId), ct);

        if (!user.IsActive)
        {
            await refreshTokens.RevokeFamilyAsync(
                rotatedToken.Token, user.Email, ct);
            Authentication.RefreshTokenCookie.Delete(HttpContext, requestedApplicationCode);
            throw new UnauthorizedAccessException("The user account is inactive.");
        }

        var application = await applications.GetByIdAsync(rotatedToken.ApplicationId, ct);
        if (application is null
            || !application.IsActive
            || !string.Equals(
                application.Code,
                requestedApplicationCode,
                StringComparison.OrdinalIgnoreCase))
        {
            await refreshTokens.RevokeFamilyAsync(rotatedToken.Token, user.Email, ct);
            Authentication.RefreshTokenCookie.Delete(HttpContext, requestedApplicationCode);
            throw new UnauthorizedAccessException("The refresh session application is unavailable.");
        }

        var authorization = await userRoles.GetAuthorizationAsync(user.Id, application.Code, ct);
        if (authorization.Roles.Count == 0)
        {
            await refreshTokens.RevokeFamilyAsync(rotatedToken.Token, user.Email, ct);
            Authentication.RefreshTokenCookie.Delete(HttpContext, requestedApplicationCode);
            throw new UnauthorizedAccessException("The user no longer has access to the application.");
        }
        var accessToken = tokens.CreateAccessToken(user, authorization, application);

        Authentication.RefreshTokenCookie.Write(
            HttpContext,
            application.Code,
            rotatedToken.Token,
            rotatedToken.ExpiresAtUtc);

        return Ok(ToResponse(accessToken));
    }
    [AllowAnonymous]
    [HttpPost("/logout")]
    public async Task<IActionResult> Logout(
        [FromQuery] string? applicationCode,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(applicationCode))
            return MissingApplicationCode();

        var requestedApplicationCode = applicationCode.Trim();
        if (Authentication.RefreshTokenCookie.TryRead(
                HttpContext,
                requestedApplicationCode,
                out var refreshToken))
        {
            await refreshTokens.RevokeFamilyAsync(
                refreshToken,
                null,
                ct);
        }

        Authentication.RefreshTokenCookie.Delete(HttpContext, requestedApplicationCode);
        return NoContent();
    }

    private ActionResult MissingApplicationCode()
    {
        ModelState.AddModelError("applicationCode", "Application code is required.");
        return ValidationProblem(ModelState);
    }

    private async Task<ApplicationDto> GetActiveApplicationAsync(
        string applicationCode,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(applicationCode))
            throw new ArgumentException("Application code is required.", nameof(applicationCode));

        var normalizedCode = applicationCode.Trim();
        var result = await applications.GetAsync(
            new ApplicationsFilter(
                Code: new StringFilter(Values: [normalizedCode]),
                IsActive: true),
            [new ApplicationsSort(ApplicationsSortColumn.Code, SortDirection.Ascending)],
            1,
            1,
            cancellationToken);
        return result.Items.Count == 0
            ? throw new NotFoundException(
                $"Application '{normalizedCode}' does not exist or is inactive.")
            : result.Items[0];
    }

    private static LoginResponse ToResponse(AccessTokenResult accessToken) =>
        new(accessToken.Token, accessToken.ExpiresAtUtc);

    public sealed record LoginRequest(
        string Code,
        string Password,
        string? ApplicationCode = null);

    public sealed record LoginResponse(
        string AccessToken,
        DateTime AccessTokenExpiresAtUtc);
}
