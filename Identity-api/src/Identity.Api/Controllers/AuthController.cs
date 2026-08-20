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
            user.Email,
            _jwtOptions.ApplicationCode,
            TimeSpan.FromDays(_jwtOptions.RefreshTokenDays),
            ct);

        Authentication.RefreshTokenCookie.Write(HttpContext, refreshToken.Token, refreshToken.ExpiresAtUtc);
        return Ok(ToResponse(accessToken));
    }

    [AllowAnonymous]
    [HttpPost("/refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(CancellationToken ct)
    {
        if (!Authentication.RefreshTokenCookie.TryRead(HttpContext, out var currentToken))
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
            await refreshTokens.RevokeAsync(
                rotatedToken.Token, user.Email, ct);
            Authentication.RefreshTokenCookie.Delete(HttpContext);
            throw new UnauthorizedAccessException("The user account is inactive.");
        }

        var authorization = await userRoles.GetAuthorizationAsync(user.Id, _jwtOptions.ApplicationCode, ct);
        var accessToken = tokens.CreateAccessToken(user, authorization);

        Authentication.RefreshTokenCookie.Write(HttpContext, rotatedToken.Token, rotatedToken.ExpiresAtUtc);

        return Ok(ToResponse(accessToken));
    }
    [HttpPost("/logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        if (Authentication.RefreshTokenCookie.TryRead(HttpContext, out var refreshToken))
        {
            await refreshTokens.RevokeAsync(
                refreshToken,
                GetCurrentUserEmail(),
                ct);
        }

        tokens.RevokeAccessToken(User);
        Authentication.RefreshTokenCookie.Delete(HttpContext);
        return NoContent();
    }

    private string? GetCurrentUserEmail() =>
        User.FindFirst("email")?.Value;

    private static LoginResponse ToResponse(AccessTokenResult accessToken) =>
        new(accessToken.Token, accessToken.ExpiresAtUtc);

    public sealed record LoginRequest(string Code, string Password);

    public sealed record LoginResponse(
        string AccessToken,
        DateTime AccessTokenExpiresAtUtc);
}
