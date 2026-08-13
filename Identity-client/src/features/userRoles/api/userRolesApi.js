import { userRolesFetch } from "./userRolesClient";

const query = (values) => new URLSearchParams(values).toString();
export const getRoleMembers = (roleId, signal) =>
  userRolesFetch(
    `/api/user-roles?${query({ roleId, page: 1, pageSize: 100 })}`,
    { signal },
  ).then((r) => r.data);
export const getRoleCandidates = (roleId, search, page, signal) =>
  userRolesFetch(
    `/api/user-roles/candidates?${query({ roleId, search, page, pageSize: 20 })}`,
    { signal },
  ).then((r) => r.data);
export const assignUsersToRole = (roleId, userIds) =>
  userRolesFetch(`/api/user-roles/${roleId}`, {
    method: "POST",
    body: JSON.stringify({ userIds }),
  }).then((r) => r.data);
export const removeUserFromRole = (roleId, userId) =>
  userRolesFetch(`/api/user-roles/${roleId}/${userId}`, { method: "DELETE" });
