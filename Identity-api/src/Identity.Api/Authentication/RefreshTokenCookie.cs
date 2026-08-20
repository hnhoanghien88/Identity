namespace Identity.Api.Authentication;

public static class RefreshTokenCookie
{
    private const string Name = "refresh_token";

    public static void Write(HttpContext context, string token, DateTime expiresAtUtc)
    {
        var options = Options(context);
        options.Expires = expiresAtUtc;
        context.Response.Cookies.Append(Name, token, options);
    }

    public static void Delete(HttpContext context) =>
        context.Response.Cookies.Delete(Name, Options(context));

    public static bool TryRead(HttpContext context, out string token)
    {
        if (context.Request.Cookies.TryGetValue(Name, out var value)
            && !string.IsNullOrWhiteSpace(value))
        {
            token = value;
            return true;
        }

        token = string.Empty;
        return false;
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
