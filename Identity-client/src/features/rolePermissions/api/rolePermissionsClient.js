import { apiFetch } from "../../../shared/api/apiClient";

export class RolePermissionsApiError extends Error {
  constructor(message, status) {
    super(message);
    this.status = status;
  }
}

export function rolePermissionsFetch(path, options = {}) {
  return apiFetch(
    path,
    options,
    (message, status) => new RolePermissionsApiError(message, status),
  );
}
