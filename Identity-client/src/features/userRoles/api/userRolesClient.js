import { clearSession, getSession, publishSession } from "../../auth/session";
import { refreshSession } from "../../auth/api/refresh";

export async function userRolesFetch(path, options = {}, retry = true) {
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
      return userRolesFetch(path, options, false);
    } catch {
      clearSession();
      throw new Error("Your session has expired. Please sign in again.");
    }
  }
  const payload = await response.json().catch(() => null);
  if (!response.ok)
    throw new Error(
      payload?.detail ||
        payload?.title ||
        "The request could not be completed.",
    );
  return payload;
}
