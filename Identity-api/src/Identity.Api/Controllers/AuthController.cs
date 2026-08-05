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
    [AllowAnonymous]
    [HttpPost("/login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var user = await sender.Send(new AuthenticateUserQuery(request.Code, request.Password), ct);
        var roles = await userRoles.GetRoleCodesAsync(user.Id, ct);
        return Ok(tokens.CreateTokens(user, roles));
    }

    [AllowAnonymous]
    [HttpPost("/refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request, CancellationToken ct)
    {
        var userId = tokens.ValidateRefreshToken(request.RefreshToken);
        var user = await sender.Send(new GetUsersByIdQuery(userId), ct);

        if (!user.IsActive)
            throw new UnauthorizedAccessException("The user account is inactive.");

        var roles = await userRoles.GetRoleCodesAsync(user.Id, ct);
        var newTokens = tokens.CreateTokens(user, roles);
        tokens.RevokeRefreshToken(request.RefreshToken);
        return Ok(newTokens);
    }

    [Authorize]
    [HttpPost("/logout")]
    public IActionResult Logout(LogoutRequest request)
    {
        tokens.RevokeRefreshToken(request.RefreshToken);
        tokens.RevokeAccessToken(User);
        return NoContent();
    }

    public sealed record LoginRequest(string Code, string Password);
    public sealed record RefreshRequest(string RefreshToken);
    public sealed record LogoutRequest(string RefreshToken);
}
