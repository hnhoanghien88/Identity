import { apiFetch } from "../../../shared/api/apiClient";

export class ActionsApiError extends Error {
  constructor(message, status, errors = {}) {
    super(message);
    this.status = status;
    this.errors = errors;
  }
}

export function actionsFetch(path, options = {}) {
  return apiFetch(
    path,
    options,
    (message, status, errors) =>
      new ActionsApiError(message, status, errors),
  );
}
