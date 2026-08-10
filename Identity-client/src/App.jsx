import { useEffect, useState } from "react";
import {
  Box,
  Button,
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
import LockOutlinedIcon from "@mui/icons-material/LockOutlined";
import LoginRoundedIcon from "@mui/icons-material/LoginRounded";
import LogoutRoundedIcon from "@mui/icons-material/LogoutRounded";
import { LoginPage } from "./features/auth";
import { clearSession, getSession, publishSession, restoreSession, subscribeToSession } from "./features/auth/session";
import { logout } from "./features/auth/api/logout";
import "./App.css";

const LOGIN_PATH = "/login";
const APPLICATIONS_PATH = "/applications";
const knownPaths = new Set([LOGIN_PATH, APPLICATIONS_PATH]);

function getCurrentPath() {
  return knownPaths.has(window.location.pathname)
    ? window.location.pathname
    : LOGIN_PATH;
}

function App() {
  const [path, setPath] = useState(getCurrentPath);
  const [authSession, setAuthSession] = useState(getSession);
  const [isRestoringSession, setIsRestoringSession] = useState(() => !getSession());
  const isLoggedIn = Boolean(authSession);

  const navigate = (nextPath, { replace = false } = {}) => {
    if (window.location.pathname !== nextPath) {
      window.history[replace ? "replaceState" : "pushState"]({}, "", nextPath);
    }
    setPath(nextPath);
  };

  useEffect(() => {
    if (!knownPaths.has(window.location.pathname)) {
      window.history.replaceState({}, "", LOGIN_PATH);
    }

    const handlePopState = () => setPath(getCurrentPath());
    window.addEventListener("popstate", handlePopState);
    return () => window.removeEventListener("popstate", handlePopState);
  }, []);

  useEffect(() => subscribeToSession((session) => {
    setAuthSession(session);

    if (session && window.location.pathname === LOGIN_PATH) {
      navigate(APPLICATIONS_PATH, { replace: true });
    } else if (!session && window.location.pathname === APPLICATIONS_PATH) {
      navigate(LOGIN_PATH, { replace: true });
    }
  }), []);

  useEffect(() => {
    if (authSession) {
      if (path === LOGIN_PATH) {
        navigate(APPLICATIONS_PATH, { replace: true });
      }
      setIsRestoringSession(false);
      return;
    }

    let isActive = true;
    setIsRestoringSession(true);

    restoreSession()
      .then((session) => {
        if (!isActive) return;
        setAuthSession(session);
        navigate(APPLICATIONS_PATH, { replace: true });
      })
      .catch(() => {
        if (isActive && path === APPLICATIONS_PATH) {
          navigate(LOGIN_PATH, { replace: true });
        }
      })
      .finally(() => {
        if (isActive) setIsRestoringSession(false);
      });

    return () => {
      isActive = false;
    };
  }, [path, authSession]);

  const handleLoginSuccess = (session) => {
    publishSession(session);
    navigate(APPLICATIONS_PATH);
  };

  const handleLogout = async () => {
    try {
      await logout(authSession?.accessToken);
    } finally {
      clearSession();
      navigate(LOGIN_PATH, { replace: true });
    }
  };

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
            disabled={!isLoggedIn}
            onClick={() => navigate(APPLICATIONS_PATH)}
          >
            <ListItemIcon>
              <AppsRoundedIcon />
            </ListItemIcon>
            <ListItemText primary="Application" />
          </ListItemButton>
        </List>
      </Paper>
      <Box component="main" className="main-content">
        {isRestoringSession ? (
          <CircularProgress aria-label="Restoring session" />
        ) : path === LOGIN_PATH ? (
          <LoginPage onLoginSuccess={handleLoginSuccess} />
        ) : (
          <Paper className="success-card" elevation={0}>
            <Box className="success-icon">
              <AppsRoundedIcon />
            </Box>
            <Typography variant="h4" component="h1" fontWeight={700}>
              Applications
            </Typography>
            <Typography color="text.secondary">
              {"Bạn đã đăng nhập thành công."}
            </Typography>
            <Button
              variant="outlined"
              color="error"
              startIcon={<LogoutRoundedIcon />}
              onClick={handleLogout}
              sx={{ mt: 3 }}
            >
              Logout
            </Button>
          </Paper>
        )}
      </Box>
    </Box>
  );
}

export default App;
