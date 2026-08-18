import { useState } from "react";
import { Alert, Box, Button, Checkbox, CircularProgress, FormControlLabel, IconButton, InputAdornment, Link, Paper, Snackbar, Stack, TextField, Typography } from "@mui/material";
import VisibilityOutlinedIcon from "@mui/icons-material/VisibilityOutlined";
import VisibilityOffOutlinedIcon from "@mui/icons-material/VisibilityOffOutlined";
import LockRoundedIcon from "@mui/icons-material/LockRounded";
import FacebookRoundedIcon from "@mui/icons-material/FacebookRounded";
import TwitterIcon from "@mui/icons-material/Twitter";
import LinkedInIcon from "@mui/icons-material/LinkedIn";
import { login } from "./api/login";

const CONNECTION_ERROR = "Kh\u00f4ng th\u1ec3 k\u1ebft n\u1ed1i \u0111\u1ebfn m\u00e1y ch\u1ee7. Vui l\u00f2ng th\u1eed l\u1ea1i.";

export function LoginPage({ onLoginSuccess }) {
  const [code, setCode] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [comingSoon, setComingSoon] = useState(false);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError("");
    setIsLoading(true);
    try {
      const session = await login({ code, password });
      onLoginSuccess(session);
    } catch (loginError) {
      setError(loginError instanceof TypeError ? CONNECTION_ERROR : loginError.message);
    } finally {
      setIsLoading(false);
    }
  };

  const showComingSoon = (event) => {
    event.preventDefault();
    setComingSoon(true);
  };

  return (
    <Box className="cuba-login-page">
      <Box className="cuba-login-decoration cuba-login-decoration-left" />
      <Box className="cuba-login-decoration cuba-login-decoration-right" />
      <Box className="cuba-login-skyline" aria-hidden="true" />
      <Paper className="cuba-login-card" elevation={0}>
        <Stack className="cuba-login-brand" direction="row" spacing={1.25}>
          <Box className="cuba-login-logo"><LockRoundedIcon fontSize="small" /></Box>
          <Box><Typography className="cuba-login-brand-name">Identity</Typography><Typography className="cuba-login-brand-caption">MANAGEMENT</Typography></Box>
        </Stack>
        <Box className="cuba-login-heading">
          <Typography variant="h4" component="h1">Sign in to account</Typography>
          <Typography color="text.secondary">Enter your code &amp; password to login</Typography>
        </Box>
        <Box component="form" onSubmit={handleSubmit} noValidate>
          <Stack spacing={2.25}>
            {error && <Alert severity="error">{error}</Alert>}
            <TextField label="Code" placeholder="Enter your code" value={code} onChange={(event) => setCode(event.target.value)} autoComplete="username" slotProps={{ htmlInput: { maxLength: 50, pattern: "[A-Za-z0-9._\\-]+" } }} required fullWidth autoFocus />
            <TextField label="Password" placeholder="Enter your password" type={showPassword ? "text" : "password"} value={password} onChange={(event) => setPassword(event.target.value)} autoComplete="current-password" slotProps={{ input: { endAdornment: <InputAdornment position="end"><IconButton aria-label={showPassword ? "Hide password" : "Show password"} onClick={() => setShowPassword((visible) => !visible)} edge="end">{showPassword ? <VisibilityOffOutlinedIcon /> : <VisibilityOutlinedIcon />}</IconButton></InputAdornment> } }} required fullWidth />
            <Stack direction="row" className="cuba-login-options" justifyContent="space-between" alignItems="center">

              <FormControlLabel control={<Checkbox size="small" />} label="Remember password" />
            </Stack>
            <Button className="cuba-login-submit" type="submit" variant="contained" size="large" disabled={isLoading || !code || !password}>{isLoading ? <CircularProgress size={24} color="inherit" /> : "Login"}</Button>
          </Stack>
        </Box>
        <Stack className="cuba-login-social" spacing={1.5}>
          <Typography component="div"><span>Or Sign in with</span></Typography>
          <Stack direction="row" spacing={1.5}>
            <Button fullWidth variant="outlined" startIcon={<FacebookRoundedIcon />} onClick={showComingSoon}>Facebook</Button>
            <Button fullWidth variant="outlined" startIcon={<TwitterIcon />} onClick={showComingSoon}>Twitter</Button>
            <Button fullWidth variant="outlined" startIcon={<LinkedInIcon />} onClick={showComingSoon}>LinkedIn</Button>
          </Stack>
        </Stack>
        <Typography className="cuba-login-register" color="text.secondary">Don&apos;t have an account?{" "}<Link href="#" underline="hover" onClick={showComingSoon}>Create Account</Link></Typography>
      </Paper>
      <Snackbar open={comingSoon} autoHideDuration={3000} onClose={() => setComingSoon(false)} message="Coming soon" anchorOrigin={{ vertical: "bottom", horizontal: "center" }} />
    </Box>
  );
}
