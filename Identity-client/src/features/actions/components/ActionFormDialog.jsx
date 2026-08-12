import { useEffect, useState } from "react";
import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Stack,
  TextField,
} from "@mui/material";

const empty = { code: "", name: "" };

export function ActionFormDialog({
  open,
  action,
  pending,
  serverError,
  fieldErrors = {},
  onClose,
  onSubmit,
}) {
  const [value, setValue] = useState(empty);
  const [errors, setErrors] = useState({});
  useEffect(() => {
    if (open) setValue(action ? { ...action } : empty);
    if (open) setErrors({});
  }, [open, action]);
  const input = (name) => ({
    value: value[name] ?? "",
    onChange: (event) => setValue({ ...value, [name]: event.target.value }),
    error: Boolean(errors[name] || fieldErrors[name]),
    helperText: errors[name] || fieldErrors[name]?.[0],
  });
  const submit = () => {
    const next = {};
    if (!value.code.trim()) next.code = "Code is required.";
    if (!value.name.trim()) next.name = "Name is required.";
    if (value.code.trim().length > 50) next.code = "Code is too long.";
    if (value.name.trim().length > 100) next.name = "Name is too long.";
    setErrors(next);
    if (Object.keys(next).length === 0) {
      onSubmit({
        code: value.code.trim(),
        name: value.name.trim(),
        ...(action ? { version: action.version } : {}),
      });
    }
  };
  return (
    <Dialog open={open} onClose={pending ? undefined : onClose} fullWidth>
      <DialogTitle>{action ? "Edit Action" : "Create Action"}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          {serverError && <Alert severity="error">{serverError}</Alert>}
          <TextField label="Code" required autoFocus {...input("code")} />
          <TextField label="Name" required {...input("name")} />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={pending}>
          Cancel
        </Button>
        <Button onClick={submit} disabled={pending} variant="contained">
          {pending ? "Saving…" : "Save"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
