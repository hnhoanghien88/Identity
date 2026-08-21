import { apiFetch } from "../../../shared/api/apiClient";

export class ResourcesApiError extends Error {
  constructor(message, status, errors = {}) {
    super(message);
    this.status = status;
    this.errors = errors;
  }
}

export function resourcesFetch(path, options = {}) {
  return apiFetch(
    path,
    options,
    (message, status, errors) =>
      new ResourcesApiError(message, status, errors),
  );
}
