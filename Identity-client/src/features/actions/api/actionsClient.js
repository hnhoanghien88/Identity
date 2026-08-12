import { clearSession, getSession, publishSession } from "../../auth/session";
import { refreshSession } from "../../auth/api/refresh";

export class ActionsApiError extends Error {
  constructor(message, status, errors = {}) {
    super(message);
    this.status = status;
    this.errors = errors;
  }
}

export async function actionsFetch(path, options = {}, retry = true) {
  const session = getSession();
  const response = await fetch(`/backend${path}`, {
    ...options,
    credentials: "include",
    headers: {
      ...(options.body ? { "Content-Type": "application/json" } : {}),
      ...(session?.accessToken
        ? { Authorization: `Bearer ${session.accessToken}` }
        : {}),
      ...options.headers,
    },
  });
  if (response.status === 401 && retry) {
    try {
      publishSession(await refreshSession());
      return actionsFetch(path, options, false);
    } catch {
      clearSession();
      throw new ActionsApiError(
        "Your session has expired. Please sign in again.",
        401,
      );
    }
  }
  const payload = await response.json().catch(() => null);
  if (!response.ok) {
    throw new ActionsApiError(
      payload?.detail ||
        payload?.title ||
        "The request could not be completed.",
      response.status,
      payload?.errors || {},
    );
  }
  return payload;
}
