import { useEffect, useState } from "react";
import {
  Box,
  CircularProgress,
  Divider,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Paper,
  Stack,
  Typography,
} from "@mui/material";
import AppsRoundedIcon from "@mui/icons-material/AppsRounded";
import BoltRoundedIcon from "@mui/icons-material/BoltRounded";
import CategoryRoundedIcon from "@mui/icons-material/CategoryRounded";
import LockOutlinedIcon from "@mui/icons-material/LockOutlined";
import LoginRoundedIcon from "@mui/icons-material/LoginRounded";
import PeopleIcon from "@mui/icons-material/People";
import AdminPanelSettingsIcon from "@mui/icons-material/AdminPanelSettings";
import AccountTreeIcon from "@mui/icons-material/AccountTree";
import { LoginPage } from "./features/auth";
import { UsersPage } from "./features/users";
import {
  ApplicationsPage,
  hasApplicationPermission,
} from "./features/applications";
import { ResourcesPage, hasResourcePermission } from "./features/resources";
import { ActionsPage } from "./features/actions";
import { RolesPage } from "./features/roles";
import { MenusPage } from "./features/menus";
import {
  canRestoreSession,
  clearSession,
  getSession,
  publishSession,
  restoreSession,
  subscribeToSession,
} from "./features/auth/session";
import { logout } from "./features/auth/api/logout";
import "./App.css";

const LOGIN_PATH = "/login";
const APPLICATIONS_PATH = "/applications";
const USERS_PATH = "/users";
const RESOURCES_PATH = "/resources";
const ACTIONS_PATH = "/actions";
const ROLES_PATH = "/roles";
const MENUS_PATH = "/menus";
const knownPaths = new Set([
  LOGIN_PATH,
  APPLICATIONS_PATH,
  RESOURCES_PATH,
  ACTIONS_PATH,
  ROLES_PATH,
  MENUS_PATH,
  USERS_PATH,
]);
const protectedPaths = new Set([
  APPLICATIONS_PATH,
  RESOURCES_PATH,
  ACTIONS_PATH,
  ROLES_PATH,
  MENUS_PATH,
  USERS_PATH,
]);
const currentPath = () =>
  knownPaths.has(window.location.pathname)
    ? window.location.pathname
    : LOGIN_PATH;
const canAccessUsers = (authSession) => Boolean(authSession?.accessToken);

function App() {
  const [path, setPath] = useState(currentPath);
  const [session, setSession] = useState(getSession);
  const [restoring, setRestoring] = useState(() => !getSession());
  const loggedIn = Boolean(session);
  const canManageUsers = canAccessUsers(session);
  const canViewApplications = hasApplicationPermission(
    session,
    "Applications.View",
  );
  const canViewResources = hasResourcePermission(session, "Resources.View");
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
          navigate(APPLICATIONS_PATH, true);
        if (!next && protectedPaths.has(window.location.pathname))
          navigate(LOGIN_PATH, true);
      }),
    [],
  );

  useEffect(() => {
    if (session) {
      if (path === LOGIN_PATH) navigate(APPLICATIONS_PATH, true);
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
          const restoredPath =
            path === ACTIONS_PATH
              ? ACTIONS_PATH
              : path === USERS_PATH && canAccessUsers(next)
                ? USERS_PATH
                : path === RESOURCES_PATH &&
                    hasResourcePermission(next, "Resources.View")
                  ? RESOURCES_PATH
                  : APPLICATIONS_PATH;
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
      await logout(session?.accessToken);
    } finally {
      clearSession();
      navigate(LOGIN_PATH, true);
    }
  };

  let content;
  if (restoring) content = <CircularProgress aria-label="Restoring session" />;
  else if (path === LOGIN_PATH)
    content = (
      <LoginPage
        onLoginSuccess={(next) => {
          publishSession(next);
          navigate(APPLICATIONS_PATH);
        }}
      />
    );
  else if (path === USERS_PATH && canManageUsers)
    content = <UsersPage session={session} />;
  else if (path === RESOURCES_PATH)
    content = <ResourcesPage session={session} />;
  else if (path === ACTIONS_PATH) content = <ActionsPage />;
  else if (path === ROLES_PATH) content = <RolesPage />;
  else if (path === MENUS_PATH) content = <MenusPage session={session} />;
  else content = <ApplicationsPage session={session} onLogout={handleLogout} />;

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
        <Divider />
        <List className="menu-list" aria-label="Main navigation">
          <ListItemButton
            selected={path === LOGIN_PATH}
            onClick={() => navigate(LOGIN_PATH)}
          >
            <ListItemIcon>
              <LoginRoundedIcon />
            </ListItemIcon>
            <ListItemText primary="Login" />
          </ListItemButton>
          <ListItemButton
            selected={path === APPLICATIONS_PATH}
            disabled={!loggedIn || !canViewApplications}
            onClick={() => navigate(APPLICATIONS_PATH)}
          >
            <ListItemIcon>
              <AppsRoundedIcon />
            </ListItemIcon>
            <ListItemText primary="Application" />
          </ListItemButton>
          <ListItemButton
            selected={path === RESOURCES_PATH}
            disabled={!loggedIn || !canViewResources}
            onClick={() => navigate(RESOURCES_PATH)}
          >
            <ListItemIcon>
              <CategoryRoundedIcon />
            </ListItemIcon>
            <ListItemText primary="Resources" />
          </ListItemButton>
          <ListItemButton
            selected={path === MENUS_PATH}
            disabled={!loggedIn}
            onClick={() => navigate(MENUS_PATH)}
          >
            <ListItemIcon>
              <AccountTreeIcon />
            </ListItemIcon>
            <ListItemText primary="Menus" />
          </ListItemButton>{" "}
          <ListItemButton
            selected={path === ROLES_PATH}
            disabled={!loggedIn}
            onClick={() => navigate(ROLES_PATH)}
          >
            <ListItemIcon>
              <AdminPanelSettingsIcon />
            </ListItemIcon>
            <ListItemText primary="Roles" />
          </ListItemButton>
          <ListItemButton
            selected={path === ACTIONS_PATH}
            disabled={!loggedIn}
            onClick={() => navigate(ACTIONS_PATH)}
          >
            <ListItemIcon>
              <BoltRoundedIcon />
            </ListItemIcon>
            <ListItemText primary="Actions" />
          </ListItemButton>{" "}
          {canManageUsers && (
            <ListItemButton
              selected={path === USERS_PATH}
              onClick={() => navigate(USERS_PATH)}
            >
              <ListItemIcon>
                <PeopleIcon />
              </ListItemIcon>
              <ListItemText primary="Users" />
            </ListItemButton>
          )}
        </List>
      </Paper>
      <Box component="main" className="main-content">
        {content}
      </Box>
    </Box>
  );
}

export default App;
