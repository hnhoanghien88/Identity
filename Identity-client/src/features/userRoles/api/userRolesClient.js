import { apiFetch } from "../../../shared/api/apiClient";

export function userRolesFetch(path, options = {}) {
  return apiFetch(path, options);
}
