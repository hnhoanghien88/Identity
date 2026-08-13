import { useState } from "react";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Paper,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { login } from "./api/login";

const CONNECTION_ERROR =
  "Kh\u00f4ng th\u1ec3 k\u1ebft n\u1ed1i \u0111\u1ebfn m\u00e1y ch\u1ee7. Vui l\u00f2ng th\u1eed l\u1ea1i.";
const LOGIN_DESCRIPTION =
  "\u0110\u0103ng nh\u1eadp \u0111\u1ec3 ti\u1ebfp t\u1ee5c v\u00e0o \u1ee9ng d\u1ee5ng.";

export function LoginPage({ onLoginSuccess }) {
  const [code, setCode] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError("");
    setIsLoading(true);

    try {
      const session = await login({
        code,
        password,
      });
      onLoginSuccess(session);
    } catch (loginError) {
      setError(
        loginError instanceof TypeError ? CONNECTION_ERROR : loginError.message,
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Paper className={"login-card"} elevation={0}>
      <Box className={"login-heading"}>
        <Typography variant={"h4"} component={"h1"} fontWeight={700}>
          Welcome back
        </Typography>
        <Typography color={"text.secondary"}>{LOGIN_DESCRIPTION}</Typography>
      </Box>
      <Box component={"form"} onSubmit={handleSubmit} noValidate>
        <Stack spacing={2.5}>
          {error && <Alert severity={"error"}>{error}</Alert>}
          <TextField
            label={"Code"}
            value={code}
            onChange={(event) => setCode(event.target.value)}
            autoComplete={"username"}
            slotProps={{
              htmlInput: {
                maxLength: 50,
                pattern: "[A-Za-z0-9._\\-]+",
              },
            }}
            required
            fullWidth
            autoFocus
          />
          <TextField
            label={"Password"}
            type={"password"}
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            autoComplete={"current-password"}
            required
            fullWidth
          />
          <Button
            type={"submit"}
            variant={"contained"}
            size={"large"}
            disabled={isLoading || !code || !password}
          >
            {isLoading ? (
              <CircularProgress size={24} color={"inherit"} />
            ) : (
              "Login"
            )}
          </Button>
        </Stack>
      </Box>
    </Paper>
  );
}
