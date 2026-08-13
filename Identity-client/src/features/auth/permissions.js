export const PERMISSION_DENIED_EVENT = "identity:permission-denied";

export const hasPermission = (session, permission) =>
  Boolean(session?.authorization?.permissions?.includes(permission));

export function runIfPermitted(session, permission, action) {
  if (hasPermission(session, permission)) {
    action();
    return true;
  }

  window.dispatchEvent(
    new CustomEvent(PERMISSION_DENIED_EVENT, { detail: { permission } }),
  );
  return false;
}
