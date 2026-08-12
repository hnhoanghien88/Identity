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
  resourceType: "",
  description: "",
  isActive: true,
};

export function ResourceFormDialog({
  open,
  resource,
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
    if (open) {
      setValue(
        resource
          ? {
              ...resource,
              applicationId: resource.applicationId,
            }
          : empty,
      );
      setErrors({});
    }
  }, [open, resource]);

  const submit = () => {
    const next = {};
    if (!value.applicationId) next.applicationId = "Application is required.";
    if (!value.code.trim()) next.code = "Code is required.";
    if (!value.name.trim()) next.name = "Name is required.";
    if (!value.resourceType.trim())
      next.resourceType = "Resource Type is required.";
    if (value.code.trim().length > 120) next.code = "Code is too long.";
    if (value.name.trim().length > 150) next.name = "Name is too long.";
    if (value.resourceType.trim().length > 30)
      next.resourceType = "Resource Type is too long.";
    if (value.description?.length > 500)
      next.description = "Description is too long.";
    setErrors(next);

    if (Object.keys(next).length === 0) {
      onSubmit({
        applicationId: Number(value.applicationId),
        code: value.code.trim(),
        name: value.name.trim(),
        resourceType: value.resourceType.trim(),
        description: value.description?.trim() || null,
        ...(resource
          ? {
              isActive: value.isActive,
              version: resource.version,
            }
          : {}),
      });
    }
  };

  const input = (name) => ({
    value: value[name] ?? "",
    onChange: (event) =>
      setValue({
        ...value,
        [name]: event.target.value,
      }),
    error: Boolean(errors[name] || fieldErrors[name]),
    helperText: errors[name] || fieldErrors[name]?.[0],
  });

  return (
    <Dialog open={open} onClose={pending ? undefined : onClose} fullWidth>
      <DialogTitle>
        {resource ? "Edit Resource" : "Create Resource"}
      </DialogTitle>
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
              <MenuItem key={application.id} value={application.id}>
                {application.code} — {application.name}
              </MenuItem>
            ))}
          </TextField>
          <TextField label="Code" required {...input("code")} />
          <TextField label="Name" required {...input("name")} />
          <TextField
            label="Resource Type"
            required
            {...input("resourceType")}
          />
          <TextField
            label="Description"
            multiline
            minRows={3}
            {...input("description")}
          />
          {resource && (
            <FormControlLabel
              control={
                <Checkbox
                  checked={value.isActive}
                  onChange={(event) =>
                    setValue({
                      ...value,
                      isActive: event.target.checked,
                    })
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
