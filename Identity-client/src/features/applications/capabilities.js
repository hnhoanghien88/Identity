export const hasApplicationPermission = (session, permission) =>
  Boolean(session?.authorization?.permissions?.includes(permission));
