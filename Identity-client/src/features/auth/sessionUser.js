const fallbackUser = {
  code: "User",
  displayName: "User",
};

export const getSessionUser = (session) => {
  const directCode = session?.user?.code ?? session?.code;
  const directDisplayName =
    session?.user?.displayName ??
    session?.user?.name ??
    session?.displayName ??
    session?.name;
  if (directCode || directDisplayName) {
    return {
      code: directCode || directDisplayName || fallbackUser.code,
      displayName: directDisplayName || directCode || fallbackUser.displayName,
    };
  }

  try {
    const payload = session?.accessToken?.split(".")[1];
    if (!payload) return fallbackUser;
    const normalized = payload.replace(/-/g, "+").replace(/_/g, "/");
    const padded = normalized.padEnd(
      normalized.length + ((4 - (normalized.length % 4)) % 4),
      "=",
    );
    const binary = atob(padded);
    const bytes = Uint8Array.from(binary, (character) =>
      character.charCodeAt(0),
    );
    const claims = JSON.parse(new TextDecoder().decode(bytes));
    const code = claims.code ?? claims.email ?? claims.unique_name;
    const displayName = claims.display_name ?? claims.name;
    return {
      code: code || displayName || fallbackUser.code,
      displayName: displayName || code || fallbackUser.displayName,
    };
  } catch {
    return fallbackUser;
  }
};

export const getSessionUserName = (session) => getSessionUser(session).code;
