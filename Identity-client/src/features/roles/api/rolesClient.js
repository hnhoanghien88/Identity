import { apiFetch } from "../../../shared/api/apiClient";

export class RolesApiError extends Error {
  constructor(message, status, errors = {}) {
    super(message);
    this.status = status;
    this.errors = errors;
  }
}

export function rolesFetch(path, options = {}) {
  return apiFetch(
    path,
    options,
    (message, status, errors) => new RolesApiError(message, status, errors),
  );
}
