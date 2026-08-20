namespace Identity.Api.Authentication;

public sealed class ExternalAuthenticationOptions
{
    public const string SectionName = "ExternalAuthentication";
    public GoogleExternalAuthenticationOptions Google { get; init; } = new();
    public string FrontendLoginUrl { get; init; } = "http://localhost:5173/login";
}

public sealed class GoogleExternalAuthenticationOptions
{
    public bool Enabled { get; init; }
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string DefaultRoleCode { get; init; } = "Viewer";
}
