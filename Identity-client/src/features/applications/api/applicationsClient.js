import { apiFetch } from "../../../shared/api/apiClient";

export class ApplicationsApiError extends Error {
  constructor(message, status, errors = {}) {
    super(message);
    this.status = status;
    this.errors = errors;
  }
}

export function applicationsFetch(path, options = {}) {
  return apiFetch(
    path,
    options,
    (message, status, errors) =>
      new ApplicationsApiError(message, status, errors),
  );
}
