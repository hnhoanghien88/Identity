import { useEffect, useState } from "react";
import {
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Divider,
  IconButton,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Paper,
  Stack,
  Typography,
} from "@mui/material";
import LockOutlinedIcon from "@mui/icons-material/LockOutlined";
import LogoutRoundedIcon from "@mui/icons-material/LogoutRounded";
import AccountTreeIcon from "@mui/icons-material/AccountTree";
import AdminPanelSettingsIcon from "@mui/icons-material/AdminPanelSettings";
import AppsIcon from "@mui/icons-material/Apps";
import BoltIcon from "@mui/icons-material/Bolt";
import CategoryIcon from "@mui/icons-material/Category";
import ManageAccountsIcon from "@mui/icons-material/ManageAccounts";
import PeopleIcon from "@mui/icons-material/People";
import SecurityIcon from "@mui/icons-material/Security";
import SpeedIcon from "@mui/icons-material/Speed";
import SearchRoundedIcon from "@mui/icons-material/SearchRounded";
import NotificationsNoneRoundedIcon from "@mui/icons-material/NotificationsNoneRounded";
import { LoginPage } from "./features/auth";
import { ForbiddenPage } from "./features/auth/ForbiddenPage";
import {
  hasPermission,
  PERMISSION_DENIED_EVENT,
} from "./features/auth/permissions";
import { UsersPage } from "./features/users";
import { ApplicationsPage } from "./features/applications";
import { ResourcesPage } from "./features/resources";
import { ActionsPage } from "./features/actions";
import { RolesPage } from "./features/roles";
import { MenusPage } from "./features/menus";
import { RolePermissionsPage } from "./features/rolePermissions";
import { UserRolesPage } from "./features/userRoles";
import { RateLimitsPage } from "./features/rateLimits";
import {
  canRestoreSession,
  clearSession,
  getSession,
  publishSession,
  restoreSession,
  subscribeToSession,
} from "./features/auth/session";
import { logout } from "./features/auth/api/logout";
import { getSessionUser } from "./features/auth/sessionUser";
import "./App.css";

const menuIcons = {
  AccountTree: AccountTreeIcon,
  AdminPanelSettings: AdminPanelSettingsIcon,
  Apps: AppsIcon,
  Bolt: BoltIcon,
  Category: CategoryIcon,
  Lock: LockOutlinedIcon,
  ManageAccounts: ManageAccountsIcon,
  People: PeopleIcon,
  Security: SecurityIcon,
  Speed: SpeedIcon,
};

function MenuIcon({ name }) {
  const Icon = menuIcons[name] || AccountTreeIcon;
  return <Icon fontSize="small" />;
}

const LOGIN_PATH = "/login";
const APPLICATIONS_PATH = "/applications";
const USERS_PATH = "/users";
const RESOURCES_PATH = "/resources";
const ACTIONS_PATH = "/actions";
const ROLES_PATH = "/roles";
const MENUS_PATH = "/menus";
const ROLE_PERMISSIONS_PATH = "/role-permissions";
const USER_ROLES_PATH = "/user-roles";
const RATE_LIMITS_PATH = "/rate-limiting";
const knownPaths = new Set([
  LOGIN_PATH,
  APPLICATIONS_PATH,
  RESOURCES_PATH,
  ACTIONS_PATH,
  ROLES_PATH,
  MENUS_PATH,
  ROLE_PERMISSIONS_PATH,
  USER_ROLES_PATH,
  RATE_LIMITS_PATH,
  USERS_PATH,
]);
const protectedPaths = new Set([
  APPLICATIONS_PATH,
  RESOURCES_PATH,
  ACTIONS_PATH,
  ROLES_PATH,
  MENUS_PATH,
  ROLE_PERMISSIONS_PATH,
  USER_ROLES_PATH,
  RATE_LIMITS_PATH,
  USERS_PATH,
]);
const readPermissions = {
  [APPLICATIONS_PATH]: "Applications.Read",
  [RESOURCES_PATH]: "Resources.Read",
  [ACTIONS_PATH]: "Actions.Read",
  [ROLES_PATH]: "Roles.Read",
  [MENUS_PATH]: "Menus.Read",
  [ROLE_PERMISSIONS_PATH]: "RolePermissions.Read",
  [USER_ROLES_PATH]: "UserRoles.Read",
  [RATE_LIMITS_PATH]: "RateLimiting.Read",
  [USERS_PATH]: "Users.Read",
};
const currentPath = () =>
  knownPaths.has(window.location.pathname)
    ? window.location.pathname
    : LOGIN_PATH;
const flattenMenus = (menus, depth = 0) =>
  (menus || []).flatMap((menu) => [
    { ...menu, depth },
    ...flattenMenus(menu.children, depth + 1),
  ]);
const firstMenuRoute = (session) =>
  flattenMenus(session?.authorization?.menus).find((menu) => menu.route)
    ?.route || LOGIN_PATH;

function App() {
  const [path, setPath] = useState(currentPath);
  const [session, setSession] = useState(getSession);
  const [restoring, setRestoring] = useState(() => !getSession());
  const [deniedPermission, setDeniedPermission] = useState("");
  useEffect(() => {
    const showPermissionDenied = (event) =>
      setDeniedPermission(event.detail?.permission || "this action");
    window.addEventListener(PERMISSION_DENIED_EVENT, showPermissionDenied);
    return () =>
      window.removeEventListener(PERMISSION_DENIED_EVENT, showPermissionDenied);
  }, []);
  const loggedIn = Boolean(session);
  const sessionUser = getSessionUser(session);
  const navigationMenus = flattenMenus(session?.authorization?.menus);
  const activeMenu = navigationMenus.find((menu) => menu.route === path);
  const pageTitle = activeMenu?.name || "Identity Management";
  const navigate = (next, replace = false) => {
    if (window.location.pathname !== next)
      window.history[replace ? "replaceState" : "pushState"]({}, "", next);
    setPath(next);
  };

  useEffect(() => {
    if (!knownPaths.has(window.location.pathname))
      window.history.replaceState({}, "", LOGIN_PATH);
    const pop = () => setPath(currentPath());
    window.addEventListener("popstate", pop);
    return () => window.removeEventListener("popstate", pop);
  }, []);

  useEffect(
    () =>
      subscribeToSession((next) => {
        setSession(next);
        if (next && window.location.pathname === LOGIN_PATH)
          navigate(firstMenuRoute(next), true);
        if (!next && protectedPaths.has(window.location.pathname))
          navigate(LOGIN_PATH, true);
      }),
    [],
  );

  useEffect(() => {
    if (session) {
      if (path === LOGIN_PATH) navigate(firstMenuRoute(session), true);
      setRestoring(false);
      return;
    }
    if (!canRestoreSession()) {
      setRestoring(false);
      if (protectedPaths.has(path)) navigate(LOGIN_PATH, true);
      return;
    }
    let active = true;
    setRestoring(true);
    restoreSession()
      .then((next) => {
        if (active && next) {
          setSession(next);
          const requiredPermission = readPermissions[path];
          const restoredPath =
            protectedPaths.has(path) &&
            (!requiredPermission || hasPermission(next, requiredPermission))
              ? path
              : firstMenuRoute(next);
          navigate(restoredPath, true);
        }
      })
      .catch(() => {
        if (active && protectedPaths.has(path)) navigate(LOGIN_PATH, true);
      })
      .finally(() => {
        if (active) setRestoring(false);
      });
    return () => {
      active = false;
    };
  }, [path, session]);

  const handleLogout = async () => {
    try {
      await logout();
    } finally {
      clearSession();
      navigate(LOGIN_PATH, true);
    }
  };

  const requiredPermission = readPermissions[path];
  const forbidden =
    Boolean(session && requiredPermission) &&
    !hasPermission(session, requiredPermission);

  let content;
  if (restoring) content = <CircularProgress aria-label="Restoring session" />;
  else if (forbidden) content = <ForbiddenPage />;
  else if (path === USERS_PATH) content = <UsersPage session={session} />;
  else if (path === RESOURCES_PATH)
    content = <ResourcesPage session={session} />;
  else if (path === ACTIONS_PATH) content = <ActionsPage session={session} />;
  else if (path === ROLES_PATH) content = <RolesPage session={session} />;
  else if (path === MENUS_PATH) content = <MenusPage session={session} />;
  else if (path === ROLE_PERMISSIONS_PATH)
    content = <RolePermissionsPage session={session} />;
  else if (path === USER_ROLES_PATH)
    content = <UserRolesPage session={session} />;
  else if (path === RATE_LIMITS_PATH)
    content = <RateLimitsPage session={session} />;
  else content = <ApplicationsPage session={session} />;

  if (restoring)
    return (
      <Box className="login-page-loading">
        <CircularProgress aria-label="Restoring session" />
      </Box>
    );

  if (path === LOGIN_PATH)
    return (
      <LoginPage
        onLoginSuccess={(next) => {
          publishSession(next);
          navigate(firstMenuRoute(next));
        }}
      />
    );

  return (
    <Box className="app-shell">
      <Paper component="aside" className="sidebar" square elevation={0}>
        <Stack
          className="brand"
          direction="row"
          sx={{ alignItems: "center" }}
          spacing={1.5}
        >
          <Box className="brand-icon">
            <LockOutlinedIcon fontSize="small" />
          </Box>
          <Box>
            <Typography variant="h6" fontWeight={700}>
              Identity
            </Typography>
            <Typography variant="caption" color="text.secondary">
              Management
            </Typography>
          </Box>
        </Stack>
        {loggedIn && (
          <Stack
            className="sidebar-account"
            direction="row"
            spacing={1}
            sx={{ alignItems: "center" }}
          >
            <Box className="sidebar-user-name">
              <Typography variant="body2">
                {sessionUser.displayName}
              </Typography>
              <Typography variant="caption">
                {sessionUser.code}
              </Typography>
            </Box>
            <Button
              size="small"
              color="error"
              startIcon={<LogoutRoundedIcon />}
              onClick={handleLogout}
            >
              Logout
            </Button>
          </Stack>
        )}
        <Divider />
        <List className="menu-list" aria-label="Main navigation">
          {loggedIn &&
            navigationMenus.map((menu) => (
              <ListItemButton
                key={menu.id}
                selected={Boolean(menu.route) && path === menu.route}
                onClick={() => menu.route && navigate(menu.route)}
                sx={{ pl: 2 + menu.depth * 2 }}
              >
                <ListItemIcon>
                  <MenuIcon name={menu.icon} />
                </ListItemIcon>
                <ListItemText primary={menu.name} />
              </ListItemButton>
            ))}{" "}
        </List>
      </Paper>
      <Box component="header" className="attex-topbar">
        <Box className="attex-topbar-left">
          <Typography className="attex-topbar-title">{pageTitle}</Typography>
          <Box className="attex-topbar-search">
            <SearchRoundedIcon fontSize="small" />
            <Typography variant="body2" sx={{ ml: 1 }}>
              Search...
            </Typography>
          </Box>
        </Box>
        <Box className="attex-topbar-right">
          <IconButton className="attex-topbar-icon" aria-label="Notifications">
            <NotificationsNoneRoundedIcon />
          </IconButton>
          <Box className="attex-user-avatar" aria-hidden="true">
            {(sessionUser.displayName || "U").charAt(0).toUpperCase()}
          </Box>
          <Box className="attex-user-meta">
            <Typography variant="body2">{sessionUser.displayName}</Typography>
            <Typography variant="caption">{sessionUser.code}</Typography>
          </Box>
        </Box>
      </Box>
      <Box component="main" className="main-content">
        {content}
      </Box>
      <Dialog
        open={Boolean(deniedPermission)}
        onClose={() => setDeniedPermission("")}
      >
        <DialogTitle>Permission required</DialogTitle>
        <DialogContent>
          <DialogContentText>
            You do not have permission to perform this action
            {deniedPermission ? ` (${deniedPermission})` : ""}.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeniedPermission("")} autoFocus>
            OK
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default App;
