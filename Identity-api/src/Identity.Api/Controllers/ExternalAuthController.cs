using System.Security.Claims;
using Identity.Api.Authentication;
using Identity.Application.Abstractions.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Identity.Api.Controllers;

[AllowAnonymous]
[ApiController]
public sealed class ExternalAuthController(
    IExternalIdentityProvisioner provisioner,
    IRefreshTokenRepository refreshTokens,
    IOptions<ExternalAuthenticationOptions> externalOptions,
    IOptions<JwtOptions> jwtOptions,
    ILogger<ExternalAuthController> logger) : ControllerBase
{
    private const string GoogleScheme = "Google";
    private const string ExternalCookieScheme = "ExternalCookie";
    private readonly ExternalAuthenticationOptions _external = externalOptions.Value;
    private readonly JwtOptions _jwt = jwtOptions.Value;

    [HttpGet("/external-login/providers")]
    public IActionResult Providers() => Ok(new { google = _external.Google.Enabled });

    [HttpGet("/external-login/google")]
    public IActionResult GoogleChallenge()
    {
        if (!_external.Google.Enabled) return NotFound();
        return Challenge(
            new AuthenticationProperties
            {
                RedirectUri = Url.ActionLink(nameof(GoogleComplete))
            },
            GoogleScheme);
    }

    [HttpGet("/external-login/google/complete")]
    public async Task<IActionResult> GoogleComplete(CancellationToken cancellationToken)
    {
        if (!_external.Google.Enabled) return NotFound();
        var authentication = await HttpContext.AuthenticateAsync(ExternalCookieScheme);
        if (!authentication.Succeeded || authentication.Principal is null)
        {
            logger.LogWarning(
                authentication.Failure,
                "Google external cookie authentication failed. None={None}; PrincipalPresent={PrincipalPresent}; PropertiesPresent={PropertiesPresent}",
                authentication.None,
                authentication.Principal is not null,
                authentication.Properties is not null);
            return RedirectToFrontend("external_auth_failed");
        }

        try
        {
            var principal = authentication.Principal;
            var profile = new ExternalIdentityProfile(
                GoogleScheme,
                RequiredClaim(principal, ClaimTypes.NameIdentifier),
                RequiredClaim(principal, ClaimTypes.Email),
                HasVerifiedEmail(principal),
                RequiredClaim(principal, ClaimTypes.Name),
                principal.FindFirstValue("picture"));
            var user = await provisioner.ProvisionAsync(
                profile,
                _jwt.ApplicationCode,
                _external.Google.DefaultRoleCode,
                cancellationToken);
            var refreshToken = await refreshTokens.IssueForLoginAsync(
                user.Id,
                user.Email,
                _jwt.ApplicationCode,
                TimeSpan.FromDays(_jwt.RefreshTokenDays),
                cancellationToken);

            RefreshTokenCookie.Write(HttpContext, refreshToken.Token, refreshToken.ExpiresAtUtc);
            return RedirectToFrontend("google", isError: false);
        }
        catch (Exception exception) when (
            exception is UnauthorizedAccessException or InvalidOperationException)
        {
            logger.LogWarning(
                exception,
                "Google external login was rejected during local account provisioning.");
            return RedirectToFrontend("external_auth_rejected", isError: true);
        }
        finally
        {
            await HttpContext.SignOutAsync(ExternalCookieScheme);
        }
    }

    private IActionResult RedirectToFrontend(string value, bool isError = true)
    {
        var separator = _external.FrontendLoginUrl.Contains('?') ? '&' : '?';
        var name = isError ? "externalError" : "external";
        return Redirect($"{_external.FrontendLoginUrl}{separator}{name}={Uri.EscapeDataString(value)}");
    }

    private static string RequiredClaim(ClaimsPrincipal principal, string claimType) =>
        principal.FindFirstValue(claimType)
        ?? throw new UnauthorizedAccessException("The external identity is incomplete.");

    private static bool HasVerifiedEmail(ClaimsPrincipal principal) =>
        principal.FindAll("email_verified")
            .Concat(principal.FindAll("verified_email"))
            .Any(claim => bool.TryParse(claim.Value, out var verified) && verified);
}
