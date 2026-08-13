export const getSessionUserName = (session) => {
  const directCode = session?.user?.code ?? session?.code;
  if (directCode) return directCode;

  try {
    const payload = session?.accessToken?.split(".")[1];
    if (!payload) return "User";
    const normalized = payload.replace(/-/g, "+").replace(/_/g, "/");
    const padded = normalized.padEnd(
      normalized.length + ((4 - (normalized.length % 4)) % 4),
      "=",
    );
    const claims = JSON.parse(atob(padded));
    return claims.code ?? claims.unique_name ?? claims.name ?? "User";
  } catch {
    return "User";
  }
};
