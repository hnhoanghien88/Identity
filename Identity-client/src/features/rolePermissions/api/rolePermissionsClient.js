import { clearSession, getSession, publishSession } from "../../auth/session";
import { refreshSession } from "../../auth/api/refresh";

export class RolePermissionsApiError extends Error {
  constructor(message, status) {
    super(message);
    this.status = status;
  }
}

export async function rolePermissionsFetch(path, options = {}, retry = true) {
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
      return rolePermissionsFetch(path, options, false);
    } catch {
      clearSession();
      throw new RolePermissionsApiError(
        "Your session has expired. Please sign in again.",
        401,
      );
    }
  }
  const payload = await response.json().catch(() => null);
  if (!response.ok) {
    throw new RolePermissionsApiError(
      payload?.detail ||
        payload?.title ||
        "The request could not be completed.",
      response.status,
    );
  }
  return payload;
}
