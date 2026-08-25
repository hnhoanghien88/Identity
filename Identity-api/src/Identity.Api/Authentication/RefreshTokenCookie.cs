using System.Security.Cryptography;
using System.Text;

namespace Identity.Api.Authentication;

public static class RefreshTokenCookie
{
    public static void Write(
        HttpContext context,
        string applicationCode,
        string token,
        DateTime expiresAtUtc)
    {
        var options = Options(context);
        options.Expires = expiresAtUtc;
        context.Response.Cookies.Append(NameFor(applicationCode), token, options);
    }

    public static void Delete(HttpContext context, string applicationCode) =>
        context.Response.Cookies.Delete(NameFor(applicationCode), Options(context));

    public static bool TryRead(
        HttpContext context,
        string applicationCode,
        out string token)
    {
        if (context.Request.Cookies.TryGetValue(NameFor(applicationCode), out var value)
            && !string.IsNullOrWhiteSpace(value))
        {
            token = value;
            return true;
        }

        token = string.Empty;
        return false;
    }

    public static string NameFor(string applicationCode)
    {
        if (string.IsNullOrWhiteSpace(applicationCode))
            throw new ArgumentException(
                "Application code is required.",
                nameof(applicationCode));

        var normalizedCode = applicationCode.Trim().ToUpperInvariant();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalizedCode));
        return $"identity_refresh_{Convert.ToHexString(hash.AsSpan(0, 16)).ToLowerInvariant()}";
    }

    private static CookieOptions Options(HttpContext context)
    {
        var isDevelopment = context.RequestServices
            .GetRequiredService<IHostEnvironment>()
            .IsDevelopment();

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDevelopment,
            SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.Strict,
            Path = "/"
        };
    }
}
