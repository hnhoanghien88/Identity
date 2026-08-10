using System.IdentityModel.Tokens.Jwt;
using Identity.Api.Authentication;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Users.AuthenticateUser;
using Identity.Application.Users.GetUsersById;
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
    IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private const string RefreshTokenCookie = "refresh_token";
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    [AllowAnonymous]
    [HttpPost("/login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken ct)
    {
        var user = await sender.Send(
            new AuthenticateUserQuery(request.Code, request.Password), ct);
        var authorization = await userRoles.GetAuthorizationAsync(user.Id, _jwtOptions.ApplicationCode, ct);
        var accessToken = tokens.CreateAccessToken(user, authorization);
        var refreshToken = await refreshTokens.IssueForLoginAsync(
            user.Id,
            user.Code,
            _jwtOptions.ApplicationCode,
            TimeSpan.FromDays(_jwtOptions.RefreshTokenDays),
            ct);

        WriteRefreshTokenCookie(refreshToken.Token, refreshToken.ExpiresAtUtc);
        return Ok(ToResponse(accessToken, authorization));
    }

    [AllowAnonymous]
    [HttpPost("/refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(CancellationToken ct)
    {
        var currentToken = ReadRefreshTokenCookie();
        var rotatedToken = await refreshTokens.RotateAsync(
            currentToken,
            TimeSpan.FromDays(_jwtOptions.RefreshTokenDays),
            ct);

        var user = await sender.Send(
            new GetUsersByIdQuery(rotatedToken.UserId), ct);

        if (!user.IsActive)
        {
            await refreshTokens.RevokeAsync(
                rotatedToken.Token, user.Code, ct);
            DeleteRefreshTokenCookie();
            throw new UnauthorizedAccessException("The user account is inactive.");
        }

        var authorization = await userRoles.GetAuthorizationAsync(user.Id, _jwtOptions.ApplicationCode, ct);
        var accessToken = tokens.CreateAccessToken(user, authorization);

        WriteRefreshTokenCookie(
            rotatedToken.Token,
            rotatedToken.ExpiresAtUtc);

        return Ok(ToResponse(accessToken, authorization));
    }

    [Authorize]
    [HttpPost("/logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        if (Request.Cookies.TryGetValue(RefreshTokenCookie, out var refreshToken)
            && !string.IsNullOrWhiteSpace(refreshToken))
        {
            await refreshTokens.RevokeAsync(
                refreshToken,
                GetCurrentUserEmail(),
                ct);
        }

        tokens.RevokeAccessToken(User);
        DeleteRefreshTokenCookie();
        return NoContent();
    }

    private string? GetCurrentUserEmail() =>
        User.FindFirst("email")?.Value;

    private string ReadRefreshTokenCookie()
    {
        if (!Request.Cookies.TryGetValue(RefreshTokenCookie, out var refreshToken)
            || string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new UnauthorizedAccessException(
                "The refresh token cookie is missing.");
        }

        return refreshToken;
    }

    private void WriteRefreshTokenCookie(
        string refreshToken,
        DateTime expiresAtUtc)
    {
        var options = RefreshCookieOptions();
        options.Expires = expiresAtUtc;
        Response.Cookies.Append(RefreshTokenCookie, refreshToken, options);
    }

    private void DeleteRefreshTokenCookie() =>
        Response.Cookies.Delete(
            RefreshTokenCookie,
            RefreshCookieOptions());

    private CookieOptions RefreshCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = Request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Path = "/"
    };

    private static LoginResponse ToResponse(
        AccessTokenResult accessToken,
        UserAuthorization authorization) =>
        new(accessToken.Token, accessToken.ExpiresAtUtc, authorization);

    public sealed record LoginRequest(string Code, string Password);

    public sealed record LoginResponse(
        string AccessToken,
        DateTime AccessTokenExpiresAtUtc,
        UserAuthorization Authorization);
}