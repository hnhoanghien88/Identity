DELETE rolePermission
FROM role_permissions AS rolePermission
INNER JOIN permissions AS permission ON permission.Id = rolePermission.PermissionId
WHERE permission.Code LIKE 'RateLimiting.%';

DELETE FROM permissions WHERE Code LIKE 'RateLimiting.%';
DELETE FROM menus WHERE Code = 'rate-limits';
