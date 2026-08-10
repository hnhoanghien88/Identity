import { clearSession, getSession, publishSession } from "../../auth/session";
import { refreshSession } from "../../auth/api/refresh";

export class UsersApiError extends Error {
  constructor(message, status, errors = {}) {
    super(message);
    this.status = status;
    this.errors = errors;
  }
}

export async function usersFetch(path, options = {}, retry = true) {
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
      return usersFetch(path, options, false);
    } catch {
      clearSession();
      throw new UsersApiError(
        "Your session has expired. Please sign in again.",
        401,
      );
    }
  }

  const payload =
    response.status === 204 ? null : await response.json().catch(() => null);
  if (!response.ok) {
    throw new UsersApiError(
      payload?.detail ||
        payload?.title ||
        "The request could not be completed.",
      response.status,
      payload?.errors || {},
    );
  }
  return payload;
}
