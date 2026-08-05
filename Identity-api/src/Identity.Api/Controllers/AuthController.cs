using Identity.Api.Authentication;
using Identity.Application.Users.AuthenticateUser;
using Identity.Application.Users.GetUsersById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
public sealed class AuthController(ISender sender, IJwtTokenService tokens) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("/login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var user = await sender.Send(new AuthenticateUserQuery(request.Code, request.Password), ct);
        return Ok(tokens.CreateTokens(user));
    }

    [AllowAnonymous]
    [HttpPost("/refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request, CancellationToken ct)
    {
        var userId = tokens.ValidateRefreshToken(request.RefreshToken);
        var user = await sender.Send(new GetUsersByIdQuery(userId), ct);

        if (!user.IsActive)
            throw new UnauthorizedAccessException("The user account is inactive.");

        var newTokens = tokens.CreateTokens(user);
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
