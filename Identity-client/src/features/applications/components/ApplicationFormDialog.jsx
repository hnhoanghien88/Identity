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
  Stack,
  TextField,
} from "@mui/material";

const empty = {
  code: "",
  name: "",
  audience: "",
  description: "",
  isActive: true,
};

export function ApplicationFormDialog({
  open,
  application,
  pending,
  serverError,
  fieldErrors = {},
  onClose,
  onSubmit,
}) {
  const [value, setValue] = useState(empty);
  const [errors, setErrors] = useState({});

  useEffect(() => {
    if (open) setValue(application ? { ...application } : empty);
    if (open) setErrors({});
  }, [open, application]);

  const submit = () => {
    const next = {};
    if (!value.code.trim()) next.code = "Code is required.";
    if (!value.name.trim()) next.name = "Name is required.";
    if (!value.audience.trim()) next.audience = "Audience is required.";
    if (value.code.trim().length > 50) next.code = "Code is too long.";
    if (value.name.trim().length > 150) next.name = "Name is too long.";
    if (value.audience.trim().length > 150)
      next.audience = "Audience is too long.";
    if (value.description?.length > 500)
      next.description = "Description is too long.";
    setErrors(next);
    if (Object.keys(next).length === 0) {
      onSubmit({
        code: value.code.trim(),
        name: value.name.trim(),
        audience: value.audience.trim(),
        description: value.description?.trim() || null,
        ...(application
          ? { isActive: value.isActive, version: application.version }
          : {}),
      });
    }
  };

  const input = (name) => ({
    value: value[name] ?? "",
    onChange: (event) => setValue({ ...value, [name]: event.target.value }),
    error: Boolean(errors[name] || fieldErrors[name]),
    helperText: errors[name] || fieldErrors[name]?.[0],
  });

  return (
    <Dialog open={open} onClose={pending ? undefined : onClose} fullWidth>
      <DialogTitle>
        {application ? "Edit Application" : "Create Application"}
      </DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          {serverError && <Alert severity="error">{serverError}</Alert>}
          <TextField label="Code" required autoFocus {...input("code")} />
          <TextField label="Name" required {...input("name")} />
          <TextField label="Audience" required {...input("audience")} />
          <TextField
            label="Description"
            multiline
            minRows={3}
            {...input("description")}
          />
          {application && (
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
          )}
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
