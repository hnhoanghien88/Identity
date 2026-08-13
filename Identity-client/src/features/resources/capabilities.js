export const hasResourcePermission = (session, permission) =>
  Boolean(session?.authorization?.permissions?.includes(permission));
