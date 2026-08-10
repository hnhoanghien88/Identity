import { useEffect, useState } from 'react'
import {
  Box, CircularProgress, Divider, List, ListItemButton, ListItemIcon,
  ListItemText, Paper, Stack, Typography,
} from '@mui/material'
import AppsRoundedIcon from '@mui/icons-material/AppsRounded'
import LockOutlinedIcon from '@mui/icons-material/LockOutlined'
import LoginRoundedIcon from '@mui/icons-material/LoginRounded'
import { LoginPage } from './features/auth'
import { refreshSession } from './features/auth/api/refresh'
import './App.css'

const LOGIN_PATH = '/login'
const APPLICATIONS_PATH = '/applications'
const knownPaths = new Set([LOGIN_PATH, APPLICATIONS_PATH])

function getCurrentPath() {
  return knownPaths.has(window.location.pathname)
    ? window.location.pathname
    : LOGIN_PATH
}

function App() {
  const [path, setPath] = useState(getCurrentPath)
  const [authSession, setAuthSession] = useState(null)
  const [isRestoringSession, setIsRestoringSession] = useState(
    getCurrentPath() === APPLICATIONS_PATH,
  )
  const isLoggedIn = Boolean(authSession)

  const navigate = (nextPath, { replace = false } = {}) => {
    if (window.location.pathname !== nextPath) {
      window.history[replace ? 'replaceState' : 'pushState']({}, '', nextPath)
    }
    setPath(nextPath)
  }

  useEffect(() => {
    if (!knownPaths.has(window.location.pathname)) {
      window.history.replaceState({}, '', LOGIN_PATH)
    }

    const handlePopState = () => setPath(getCurrentPath())
    window.addEventListener('popstate', handlePopState)
    return () => window.removeEventListener('popstate', handlePopState)
  }, [])

  useEffect(() => {
    if (path !== APPLICATIONS_PATH || authSession) {
      setIsRestoringSession(false)
      return
    }

    let isActive = true
    setIsRestoringSession(true)

    refreshSession()
      .then((session) => {
        if (isActive) setAuthSession(session)
      })
      .catch(() => {
        if (isActive) navigate(LOGIN_PATH, { replace: true })
      })
      .finally(() => {
        if (isActive) setIsRestoringSession(false)
      })

    return () => {
      isActive = false
    }
  }, [path, authSession])

  const handleLoginSuccess = (session) => {
    setAuthSession(session)
    navigate(APPLICATIONS_PATH)
  }

  return (
    <Box className="app-shell">
      <Paper component="aside" className="sidebar" square elevation={0}>
        <Stack className="brand" direction="row" alignItems="center" spacing={1.5}>
          <Box className="brand-icon"><LockOutlinedIcon fontSize="small" /></Box>
          <Box>
            <Typography variant="h6" fontWeight={700}>Identity</Typography>
            <Typography variant="caption" color="text.secondary">Management</Typography>
          </Box>
        </Stack>
        <Divider />
        <List className="menu-list" aria-label="Main navigation">
          <ListItemButton selected={path === LOGIN_PATH} onClick={() => navigate(LOGIN_PATH)}>
            <ListItemIcon><LoginRoundedIcon /></ListItemIcon>
            <ListItemText primary="Login" />
          </ListItemButton>
          <ListItemButton selected={path === APPLICATIONS_PATH} disabled={!isLoggedIn}
            onClick={() => navigate(APPLICATIONS_PATH)}>
            <ListItemIcon><AppsRoundedIcon /></ListItemIcon>
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
            <Box className="success-icon"><AppsRoundedIcon /></Box>
            <Typography variant="h4" component="h1" fontWeight={700}>Applications</Typography>
            <Typography color="text.secondary">
              {'Bạn đã đăng nhập thành công.'}
            </Typography>
          </Paper>
        )}
      </Box>
    </Box>
  )
}

export default App