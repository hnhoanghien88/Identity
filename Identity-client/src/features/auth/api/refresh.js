import { applicationCode, getAuthorization } from "./authorization";

let pendingRefresh = null;

export function refreshSession() {
  if (!pendingRefresh) {
    pendingRefresh = requestRefresh().finally(() => {
      pendingRefresh = null;
    });
  }

  return pendingRefresh;
}

async function requestRefresh() {
  if (!applicationCode) {
    throw new Error("VITE_APPLICATION_CODE is required.");
  }
  const query = new URLSearchParams({ applicationCode });
  const response = await fetch(`/backend/refresh?${query}`, {
    method: "POST",
    credentials: "include",
  });

  if (!response.ok) {
    const problem = await response.json().catch(() => null);
    throw new Error(problem?.detail || "Unable to restore the session.");
  }

  const session = await response.json();
  const authorization = await getAuthorization(session.accessToken);
  return { ...session, authorization };
}

