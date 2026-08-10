import { useEffect, useState } from "react";
import {
  Alert,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Stack,
  TextField,
} from "@mui/material";

const empty = {
  code: "",
  email: "",
  name: "",
  password: "",
};

export function UserFormDialog({
  open,
  user,
  pending,
  serverError,
  fieldErrors = {},
  onClose,
  onSubmit,
}) {
  const [form, setForm] = useState(empty);
  useEffect(() => {
    if (open)
      setForm(
        user
          ? {
              code: user.code,
              email: user.email,
              name: user.name,
              password: "",
            }
          : empty,
      );
  }, [open, user]);
  const change = (field) => (event) =>
    setForm((current) => ({ ...current, [field]: event.target.value }));
  const submit = async (event) => {
    event.preventDefault();
    try {
      await onSubmit(
        user
          ? {
              code: form.code,
              email: form.email,
              name: form.name,
              version: user.version,
            }
          : form,
      );
    } finally {
      if (!user) setForm((current) => ({ ...current, password: "" }));
    }
  };
  return (
    <Dialog
      open={open}
      onClose={pending ? undefined : onClose}
      fullWidth
      maxWidth="sm"
    >
      <Stack component="form" onSubmit={submit}>
        <DialogTitle>{user ? "Edit user" : "Create user"}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ pt: 1 }}>
            {serverError && <Alert severity="error">{serverError}</Alert>}
            <TextField
              autoFocus
              required
              label="Code"
              value={form.code}
              onChange={change("code")}
              error={Boolean(fieldErrors.code)}
              helperText={fieldErrors.code?.[0]}
              slotProps={{
                htmlInput: {
                  maxLength: 50,
                  pattern: "[A-Za-z0-9._-]+",
                },
              }}
            />
            <TextField
              required
              type="email"
              label="Email"
              value={form.email}
              onChange={change("email")}
              error={Boolean(fieldErrors.email)}
              helperText={fieldErrors.email?.[0]}
              slotProps={{
                htmlInput: {
                  maxLength: 254,
                },
              }}
            />
            <TextField
              required
              label="Display name"
              value={form.name}
              onChange={change("name")}
              error={Boolean(fieldErrors.name)}
              helperText={fieldErrors.name?.[0]}
              slotProps={{
                htmlInput: {
                  maxLength: 200,
                },
              }}
            />
            {!user && (
              <TextField
                required
                type="password"
                label="Initial password"
                value={form.password}
                onChange={change("password")}
                error={Boolean(fieldErrors.password)}
                helperText={fieldErrors.password?.[0] || "8 to 128 characters"}
                slotProps={{
                  htmlInput: {
                    minLength: 8,
                    maxLength: 128,
                  },
                }}
                autoComplete="new-password"
              />
            )}
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose} disabled={pending}>
            Cancel
          </Button>
          <Button type="submit" variant="contained" disabled={pending}>
            {pending ? <CircularProgress size={22} /> : "Save"}
          </Button>
        </DialogActions>
      </Stack>
    </Dialog>
  );
}
