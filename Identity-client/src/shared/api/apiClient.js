import {
  clearSession,
  getSession,
  refreshSessionCoordinated,
} from "../../features/auth/session";

const DEFAULT_ERROR_MESSAGE = "The request could not be completed.";
const SESSION_EXPIRED_MESSAGE =
  "Your session has expired. Please sign in again.";

export async function apiFetch(path, options = {}, createError = defaultError) {
  return send(path, options, createError, true);
}

async function send(path, options, createError, retry) {
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
      await refreshSessionCoordinated(session?.accessToken);
    } catch {
      clearSession();
      throw createError(SESSION_EXPIRED_MESSAGE, 401);
    }

    return send(path, options, createError, false);
  }

  const payload =
    response.status === 204 ? null : await response.json().catch(() => null);

  if (!response.ok) {
    throw createError(
      payload?.detail || payload?.title || DEFAULT_ERROR_MESSAGE,
      response.status,
      payload?.errors || {},
    );
  }

  return payload;
}

function defaultError(message) {
  return new Error(message);
}
