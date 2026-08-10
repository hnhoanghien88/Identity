using Identity.Api.Authentication;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Users.AuthenticateUser;
using Identity.Application.Users.GetUsersById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
public sealed class AuthController(
    ISender sender,
    IJwtTokenService tokens,
    IUserRolesReadRepository userRoles) : ControllerBase
{
    private const string RefreshTokenCookie = "refresh_token";

    [AllowAnonymous]
    [HttpPost("/login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken ct)
    {
        var user = await sender.Send(
            new AuthenticateUserQuery(request.Code, request.Password), ct);
        var authorization = await userRoles.GetAuthorizationAsync(user.Id, ct);
        var loginTokens = tokens.CreateTokens(user, authorization);

        WriteRefreshTokenCookie(loginTokens);
        return Ok(ToResponse(loginTokens, authorization));
    }

    [AllowAnonymous]
    [HttpPost("/refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(CancellationToken ct)
    {
        var refreshToken = ReadRefreshTokenCookie();
        var userId = tokens.ValidateRefreshToken(refreshToken);
        var user = await sender.Send(new GetUsersByIdQuery(userId), ct);

        if (!user.IsActive)
            throw new UnauthorizedAccessException("The user account is inactive.");

        var authorization = await userRoles.GetAuthorizationAsync(user.Id, ct);
        var newTokens = tokens.CreateTokens(user, authorization);

        tokens.RevokeRefreshToken(refreshToken);
        WriteRefreshTokenCookie(newTokens);

        return Ok(ToResponse(newTokens, authorization));
    }

    [Authorize]
    [HttpPost("/logout")]
    public IActionResult Logout()
    {
        if (Request.Cookies.TryGetValue(RefreshTokenCookie, out var refreshToken)
            && !string.IsNullOrWhiteSpace(refreshToken))
        {
            tokens.RevokeRefreshToken(refreshToken);
        }

        tokens.RevokeAccessToken(User);
        Response.Cookies.Delete(RefreshTokenCookie, RefreshCookieOptions());
        return NoContent();
    }

    private string ReadRefreshTokenCookie()
    {
        if (!Request.Cookies.TryGetValue(RefreshTokenCookie, out var refreshToken)
            || string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new UnauthorizedAccessException("The refresh token cookie is missing.");
        }

        return refreshToken;
    }

    private void WriteRefreshTokenCookie(LoginTokens loginTokens)
    {
        var options = RefreshCookieOptions();
        options.Expires = loginTokens.RefreshTokenExpiresAtUtc;
        Response.Cookies.Append(
            RefreshTokenCookie,
            loginTokens.RefreshToken,
            options);
    }

    private CookieOptions RefreshCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = Request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Path = "/"
    };

    private static LoginResponse ToResponse(
        LoginTokens loginTokens,
        UserAuthorization authorization) =>
        new(
            loginTokens.AccessToken,
            loginTokens.AccessTokenExpiresAtUtc,
            authorization);

    public sealed record LoginRequest(string Code, string Password);

    public sealed record LoginResponse(
        string AccessToken,
        DateTime AccessTokenExpiresAtUtc,
        UserAuthorization Authorization);
}