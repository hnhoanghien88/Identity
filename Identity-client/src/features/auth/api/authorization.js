export const applicationCode = import.meta.env.VITE_APPLICATION_CODE?.trim();

export async function getAuthorization(accessToken) {
  if (!applicationCode) {
    throw new Error("VITE_APPLICATION_CODE is required.");
  }
  const query = new URLSearchParams({ applicationCode });
  const response = await fetch(`/backend/authorization?${query}`, {
    credentials: "include",
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  });

  if (!response.ok) {
    const problem = await response.json().catch(() => null);
    throw new Error(problem?.detail || "Unable to load user permissions.");
  }

  return response.json();
}
