export async function getAuthorization(accessToken) {
  const response = await fetch("/backend/authorization", {
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
