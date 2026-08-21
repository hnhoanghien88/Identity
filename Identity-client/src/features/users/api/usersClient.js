import { apiFetch } from "../../../shared/api/apiClient";

export class UsersApiError extends Error {
  constructor(message, status, errors = {}) {
    super(message);
    this.status = status;
    this.errors = errors;
  }
}

export function usersFetch(path, options = {}) {
  return apiFetch(
    path,
    options,
    (message, status, errors) => new UsersApiError(message, status, errors),
  );
}
