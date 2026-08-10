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
  const response = await fetch("/backend/refresh", {
    method: "POST",
    credentials: "include",
  });

  if (!response.ok) {
    const problem = await response.json().catch(() => null);
    throw new Error(problem?.detail || "Unable to restore the session.");
  }

  return response.json();
}
