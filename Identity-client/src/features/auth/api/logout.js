import { applicationCode } from "./authorization";

export async function logout() {
  if (!applicationCode) {
    throw new Error("VITE_APPLICATION_CODE is required.");
  }
  const query = new URLSearchParams({ applicationCode });
  const response = await fetch(`/backend/logout?${query}`, {
    method: "POST",
    credentials: "include",
  });

  if (!response.ok && response.status !== 401) {
    throw new Error("Unable to log out.");
  }
}
