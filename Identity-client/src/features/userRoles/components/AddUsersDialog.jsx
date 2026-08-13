import { useEffect, useState } from "react";
import {
  Alert,
  Button,
  Checkbox,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  List,
  ListItemButton,
  ListItemText,
  Pagination,
  Stack,
  TextField,
} from "@mui/material";
import { getRoleCandidates } from "../api/userRolesApi";

export function AddUsersDialog({
  role,
  open,
  pending,
  error,
  onClose,
  onSave,
}) {
  const [items, setItems] = useState([]);
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pages, setPages] = useState(1);
  const [selected, setSelected] = useState(new Set());
  const [loading, setLoading] = useState(false);
  useEffect(() => {
    if (!open || !role) return;
    const controller = new AbortController();
    setLoading(true);
    getRoleCandidates(role.id, search, page, controller.signal)
      .then((data) => {
        setItems(data.items);
        setPages(Math.max(1, Math.ceil(data.totalCount / data.pageSize)));
      })
      .finally(() => setLoading(false));
    return () => controller.abort();
  }, [open, role, search, page]);
  const toggle = (id) =>
    setSelected((current) => {
      const next = new Set(current);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  return (
    <Dialog
      open={open}
      onClose={pending ? undefined : onClose}
      fullWidth
      maxWidth="sm"
    >
      <DialogTitle>Add Users to {role?.code}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ pt: 1 }}>
          {error && <Alert severity="error">{error}</Alert>}
          <TextField
            label="Search Users"
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
              setPage(1);
            }}
          />
          {loading ? (
            <CircularProgress aria-label="Loading Users" />
          ) : (
            <List aria-label="Users available to add">
              {items.map((user) => (
                <ListItemButton
                  key={user.userId}
                  onClick={() => toggle(user.userId)}
                >
                  <Checkbox
                    checked={selected.has(user.userId)}
                    inputProps={{ "aria-label": `Select ${user.code}` }}
                  />
                  <ListItemText
                    primary={user.code}
                    secondary={`${user.name} · ${user.email}`}
                  />
                </ListItemButton>
              ))}
            </List>
          )}
          <Pagination
            page={page}
            count={pages}
            onChange={(_, value) => setPage(value)}
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={pending}>
          Cancel
        </Button>
        <Button
          variant="contained"
          disabled={pending || selected.size === 0}
          onClick={() => onSave([...selected])}
        >
          {pending ? <CircularProgress size={22} /> : "Save"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
