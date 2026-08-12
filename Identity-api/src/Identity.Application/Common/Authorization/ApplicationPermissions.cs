namespace Identity.Application.Common.Authorization;

public static class ApplicationPermissions
{
    public const string View = "Applications.View";
    public const string Create = "Applications.Create";
    public const string Update = "Applications.Update";
    public const string Delete = "Applications.Delete";
    public static readonly string[] All = [View, Create, Update, Delete];
}
