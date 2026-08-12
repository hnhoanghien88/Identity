import { rolePermissionsFetch } from "./rolePermissionsClient";

export const getRolePermissions = (roleId, resourceId, signal) =>
  rolePermissionsFetch(
    `/api/role-permissions?roleId=${roleId}&resourceId=${resourceId}`,
    { signal },
  ).then((response) => response.data);

export const grantRolePermission = (roleId, resourceId, actionId) =>
  rolePermissionsFetch(
    `/api/role-permissions/${roleId}/${resourceId}/${actionId}`,
    { method: "PUT" },
  );

export const revokeRolePermission = (roleId, resourceId, actionId) =>
  rolePermissionsFetch(
    `/api/role-permissions/${roleId}/${resourceId}/${actionId}`,
    { method: "DELETE" },
  );
