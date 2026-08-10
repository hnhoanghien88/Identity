using System.Text.RegularExpressions;

namespace Identity.Application.Users;

public static partial class UserIdentityRules
{
    public const int CodeMaximumLength = 50;
    public const int EmailMaximumLength = 254;

    public static bool IsValidCode(string code) =>
        !string.IsNullOrEmpty(code)
        && CodeRegex().IsMatch(code);

    public static string CleanEmail(string email) =>
        email.Trim();

    [GeneratedRegex("^[A-Za-z0-9._-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex CodeRegex();
}
