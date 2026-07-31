namespace Identity.Application.Users.GetUsers;

public sealed record StringFilter(
    IReadOnlyCollection<string>? Values = null,
    string? Contains = null,
    string? StartsWith = null,
    string? EndsWith = null);
