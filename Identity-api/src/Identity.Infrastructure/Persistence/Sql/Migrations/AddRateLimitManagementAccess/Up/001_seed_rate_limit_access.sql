INSERT INTO permissions
    (ResourceId, ActionId, Code, Name, CreatedBy, CreatedDate)
SELECT resource.Id, action.Id, CONCAT('RateLimiting.', action.Code), action.Name,
       'migration', UTC_TIMESTAMP(6)
FROM resources AS resource
INNER JOIN applications AS application ON application.Id = resource.ApplicationId
INNER JOIN permission_actions AS action
    ON action.Code IN ('Read', 'Create', 'Update', 'Delete', 'ViewMenu')
WHERE application.Code = 'Identity-api'
    AND resource.Code = 'RateLimiting'
    AND resource.IsDeleted = 0
    AND NOT EXISTS (
        SELECT 1 FROM permissions
        WHERE ResourceId = resource.Id AND ActionId = action.Id
    );

INSERT INTO role_permissions
    (RoleId, PermissionId, CreatedBy, CreatedDate)
SELECT DISTINCT existing.RoleId, target.Id, 'migration', UTC_TIMESTAMP(6)
FROM role_permissions AS existing
INNER JOIN permissions AS source ON source.Id = existing.PermissionId
INNER JOIN permissions AS target
    ON target.Code = CONCAT('RateLimiting.', SUBSTRING_INDEX(source.Code, '.', -1))
WHERE source.Code IN (
    'Menus.Read', 'Menus.Create', 'Menus.Update', 'Menus.Delete', 'Menus.ViewMenu'
)
AND NOT EXISTS (
    SELECT 1 FROM role_permissions
    WHERE RoleId = existing.RoleId AND PermissionId = target.Id
);
