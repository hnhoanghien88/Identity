import { rolesFetch } from "./rolesClient";

export const searchRoles = (request, signal) =>
  rolesFetch("/api/roles/search", {
    method: "POST",
    body: JSON.stringify(request),
    signal,
  }).then((response) => response.data);
export const createRole = (role) =>
  rolesFetch("/api/roles", {
    method: "POST",
    body: JSON.stringify(role),
  }).then((response) => response.data);
export const updateRole = (id, role) =>
  rolesFetch(`/api/roles/${id}`, {
    method: "PUT",
    body: JSON.stringify(role),
  }).then((response) => response.data);
export const deleteRole = (id, version) =>
  rolesFetch(`/api/roles/${id}?version=${version}`, { method: "DELETE" });
