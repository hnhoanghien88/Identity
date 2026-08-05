using Identity.Api.Authentication;
using Identity.Application.Users.AuthenticateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
public sealed class AuthController(ISender sender, IJwtTokenService tokens) : ControllerBase
{
    [HttpPost("/login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var user = await sender.Send(new AuthenticateUserQuery(request.Code, request.Password), ct);
        return Ok(tokens.CreateTokens(user));
    }
    public sealed record LoginRequest(string Code, string Password);
}
