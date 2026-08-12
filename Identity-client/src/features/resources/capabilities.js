export const hasResourcePermission = (session) =>
  Boolean(session?.accessToken);
