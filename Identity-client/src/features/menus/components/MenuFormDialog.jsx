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
  parentId: "",
  resourceId: "",
  code: "",
  name: "",
  route: "",
  icon: "",
  sortOrder: 0,
  isVisible: false,
  isActive: true,
};

const flatten = (nodes, depth = 0) =>
  nodes.flatMap((node) => [
    { ...node, depth },
    ...flatten(node.children || [], depth + 1),
  ]);

export function MenuFormDialog({
  open,
  menu,
  applicationId,
  menus,
  resources,
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
        menu
          ? {
              ...menu,
              parentId: menu.parentId ?? "",
              resourceId: menu.resourceId ?? "",
              route: menu.route ?? "",
              icon: menu.icon ?? "",
            }
          : empty,
      );
      setErrors({});
    }
  }, [open, menu]);
  const descendants = new Set();
  const collect = (nodes, inside = false) =>
    nodes.forEach((node) => {
      const next = inside || node.id === menu?.id;
      if (inside) descendants.add(node.id);
      collect(node.children || [], next);
    });
  collect(menus);
  const parents = flatten(menus).filter(
    (item) => item.id !== menu?.id && !descendants.has(item.id),
  );
  const input = (name) => ({
    value: value[name] ?? "",
    onChange: (event) => setValue({ ...value, [name]: event.target.value }),
    error: Boolean(errors[name] || fieldErrors[name]),
    helperText: errors[name] || fieldErrors[name]?.[0],
  });
  const submit = () => {
    const next = {};
    if (!value.code?.trim()) next.code = "Code is required.";
    if (!value.name?.trim()) next.name = "Name is required.";
    setErrors(next);
    if (Object.keys(next).length) return;
    onSubmit({
      applicationId,
      parentId: value.parentId ? Number(value.parentId) : null,
      resourceId: value.resourceId ? Number(value.resourceId) : null,
      code: value.code.trim(),
      name: value.name.trim(),
      route: value.route?.trim() || null,
      icon: value.icon?.trim() || null,
      sortOrder: Number(value.sortOrder),
      isVisible: value.isVisible,
      isActive: value.isActive,
      ...(menu ? { version: menu.version } : {}),
    });
  };
  return (
    <Dialog open={open} onClose={pending ? undefined : onClose} fullWidth>
      <DialogTitle>{menu ? "Edit Menu" : "Create Menu"}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          {serverError && <Alert severity="error">{serverError}</Alert>}
          <TextField select label="Parent Menu" {...input("parentId")}>
            <MenuItem value="">Root</MenuItem>
            {parents.map((item) => (
              <MenuItem key={item.id} value={item.id}>
                {"— ".repeat(item.depth)}
                {item.name}
              </MenuItem>
            ))}
          </TextField>
          <TextField select label="Resource" {...input("resourceId")}>
            <MenuItem value="">None</MenuItem>
            {resources.map((item) => (
              <MenuItem key={item.id} value={item.id}>
                {item.code} — {item.name}
              </MenuItem>
            ))}
          </TextField>
          <TextField
            label="Code"
            required
            autoFocus
            {...input("code")}
            inputProps={{ maxLength: 120 }}
          />
          <TextField
            label="Name"
            required
            {...input("name")}
            inputProps={{ maxLength: 150 }}
          />
          <TextField
            label="Route"
            {...input("route")}
            inputProps={{ maxLength: 300 }}
          />
          <TextField
            label="Icon"
            {...input("icon")}
            inputProps={{ maxLength: 100 }}
          />
          <TextField label="Sort order" type="number" {...input("sortOrder")} />
          <FormControlLabel
            control={
              <Checkbox
                checked={value.isVisible}
                onChange={(event) =>
                  setValue({ ...value, isVisible: event.target.checked })
                }
              />
            }
            label="Hidden"
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
