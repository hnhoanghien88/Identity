using Identity.Application.Actions.Dtos;
using ActionEntity = Identity.Domain.Entities.PermissionActions;

namespace Identity.Application.Actions;

public static class ActionRules
{
    public static string Clean(string value) => value.Trim();
    public static ActionDto ToDto(ActionEntity value) =>
        new(value.Id, value.Code, value.Name, value.CreatedDate, value.Version);
}

