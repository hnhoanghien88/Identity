export const hasApplicationPermission = (session) =>
  Boolean(session?.accessToken);
