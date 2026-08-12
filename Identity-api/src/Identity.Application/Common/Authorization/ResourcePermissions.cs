namespace Identity.Application.Common.Authorization;

public static class ResourcePermissions
{
    public const string View = "Resources.View";
    public const string Create = "Resources.Create";
    public const string Update = "Resources.Update";
    public const string Delete = "Resources.Delete";
    public static readonly string[] All = [View, Create, Update, Delete];
}
