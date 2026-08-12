import { useEffect, useState } from "react";
import {
  Alert,
  Button,
  Checkbox,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControlLabel,
  MenuItem,
  Stack,
  TextField,
} from "@mui/material";

const empty = {
  applicationId: "",
  code: "",
  name: "",
  isSystemRole: false,
  isActive: true,
};

export function RoleFormDialog({
  open,
  role,
  applications,
  pending,
  serverError,
  fieldErrors = {},
  onClose,
  onSubmit,
}) {
  const [value, setValue] = useState(empty);
  const [errors, setErrors] = useState({});
  useEffect(() => {
    if (open)
      setValue(
        role ? { ...role, applicationId: String(role.applicationId) } : empty,
      );
    if (open) setErrors({});
  }, [open, role]);
  const input = (name) => ({
    value: value[name] ?? "",
    onChange: (event) => setValue({ ...value, [name]: event.target.value }),
    error: Boolean(errors[name] || fieldErrors[name]),
    helperText: errors[name] || fieldErrors[name]?.[0],
  });
  const submit = () => {
    const next = {};
    if (!value.applicationId) next.applicationId = "Application is required.";
    if (!value.code.trim()) next.code = "Code is required.";
    if (!value.name.trim()) next.name = "Name is required.";
    if (value.code.trim().length > 100) next.code = "Code is too long.";
    if (value.name.trim().length > 150) next.name = "Name is too long.";
    setErrors(next);
    if (Object.keys(next).length === 0) {
      onSubmit({
        applicationId: Number(value.applicationId),
        code: value.code.trim(),
        name: value.name.trim(),
        isSystemRole: value.isSystemRole,
        isActive: value.isActive,
        ...(role ? { version: role.version } : {}),
      });
    }
  };
  return (
    <Dialog open={open} onClose={pending ? undefined : onClose} fullWidth>
      <DialogTitle>{role ? "Edit Role" : "Create Role"}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          {serverError && <Alert severity="error">{serverError}</Alert>}
          <TextField
            select
            label="Application"
            required
            autoFocus
            {...input("applicationId")}
          >
            {applications.map((application) => (
              <MenuItem key={application.id} value={String(application.id)}>
                {application.code} — {application.name}
              </MenuItem>
            ))}
          </TextField>
          <TextField label="Code" required {...input("code")} />
          <TextField label="Name" required {...input("name")} />
          <FormControlLabel
            control={
              <Checkbox
                checked={value.isSystemRole}
                onChange={(event) =>
                  setValue({ ...value, isSystemRole: event.target.checked })
                }
              />
            }
            label="System Role"
          />
          <FormControlLabel
            control={
              <Checkbox
                checked={value.isActive}
                onChange={(event) =>
                  setValue({ ...value, isActive: event.target.checked })
                }
              />
            }
            label="Active"
          />
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
