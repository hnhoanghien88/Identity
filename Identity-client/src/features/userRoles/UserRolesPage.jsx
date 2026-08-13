import { useCallback, useEffect, useRef, useState } from "react";
import { Add, Delete } from "@mui/icons-material";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Paper,
  Snackbar,
  Stack,
  Typography,
} from "@mui/material";
import { runIfPermitted } from "../auth/permissions";
import { searchRoles } from "../roles/api/rolesApi";
import {
  assignUsersToRole,
  getRoleMembers,
  removeUserFromRole,
} from "./api/userRolesApi";
import { AddUsersDialog } from "./components/AddUsersDialog";
import { RemoveUserDialog } from "./components/RemoveUserDialog";

export function UserRolesPage({ session }) {
  const [roles, setRoles] = useState([]);
  const [roleId, setRoleId] = useState(null);
  const [members, setMembers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [addOpen, setAddOpen] = useState(false);
  const [removeUser, setRemoveUser] = useState(null);
  const [pending, setPending] = useState(false);
  const [dialogError, setDialogError] = useState("");
  const [notice, setNotice] = useState("");
  const [reload, setReload] = useState(0);
  const generation = useRef(0);
  const activeRole = roles.find((role) => role.id === roleId);

  useEffect(() => {
    const controller = new AbortController();
    searchRoles(
      {
        filter: { isActive: true },
        sorts: [{ column: 2, direction: 0 }],
        page: 1,
        pageSize: 100,
      },
      controller.signal,
    )
      .then((data) => {
        setRoles(data.items);
        setRoleId((current) => current ?? data.items[0]?.id ?? null);
      })
      .catch(
        (reason) => reason.name !== "AbortError" && setError(reason.message),
      );
    return () => controller.abort();
  }, []);

  const loadMembers = useCallback(() => {
    if (!roleId) {
      setMembers([]);
      setLoading(false);
      return () => {};
    }
    const controller = new AbortController();
    const current = ++generation.current;
    setLoading(true);
    setError("");
    getRoleMembers(roleId, controller.signal)
      .then((data) => current === generation.current && setMembers(data.items))
      .catch(
        (reason) =>
          reason.name !== "AbortError" &&
          current === generation.current &&
          setError(reason.message),
      )
      .finally(() => current === generation.current && setLoading(false));
    return () => controller.abort();
  }, [roleId]);
  useEffect(() => loadMembers(), [loadMembers, reload]);

  const assign = async (userIds) => {
    setPending(true);
    setDialogError("");
    try {
      await assignUsersToRole(roleId, userIds);
      setAddOpen(false);
      setNotice("Users added to Role.");
      setReload((value) => value + 1);
    } catch (reason) {
      setDialogError(reason.message);
    } finally {
      setPending(false);
    }
  };
  const remove = async () => {
    setPending(true);
    setDialogError("");
    try {
      await removeUserFromRole(roleId, removeUser.userId);
      setRemoveUser(null);
      setNotice("User removed from Role.");
      setReload((value) => value + 1);
    } catch (reason) {
      setDialogError(reason.message);
    } finally {
      setPending(false);
    }
  };

  return (
    <Box className="user-roles-page">
      <Stack spacing={3}>
        <Box>
          <Typography component="h1" variant="h4" fontWeight={700}>
            User Roles
          </Typography>
          <Typography color="text.secondary">
            Manage Users assigned to each Role.
          </Typography>
        </Box>
        {error && <Alert severity="error">{error}</Alert>}
        <Box className="user-roles-grid">
          <Paper variant="outlined" className="user-roles-column">
            <Typography component="h2" variant="h6" fontWeight={700}>
              Roles
            </Typography>
            <List aria-label="Roles">
              {roles.map((role) => (
                <ListItemButton
                  key={role.id}
                  selected={role.id === roleId}
                  aria-current={role.id === roleId ? "true" : undefined}
                  onClick={() => setRoleId(role.id)}
                >
                  <ListItemText primary={role.code} secondary={role.name} />
                </ListItemButton>
              ))}
            </List>
          </Paper>
          <Paper variant="outlined" className="user-roles-column">
            <Stack
              direction="row"
              sx={{
                alignItems: "center",
                justifyContent: "space-between",
              }}
            >
              <Typography component="h2" variant="h6" fontWeight={700}>
                Users in {activeRole?.code ?? "Role"}
              </Typography>
              <Button
                startIcon={<Add />}
                disabled={!roleId}
                onClick={() =>
                  runIfPermitted(session, "UserRoles.Create", () => {
                    setDialogError("");
                    setAddOpen(true);
                  })
                }
              >
                Add User
              </Button>
            </Stack>
            {loading ? (
              <CircularProgress aria-label="Loading Role Users" />
            ) : members.length === 0 ? (
              <Alert severity="info">No Users belong to this Role.</Alert>
            ) : (
              <List aria-label="Users in active Role">
                {members.map((user) => (
                  <ListItem
                    key={user.userId}
                    secondaryAction={
                      <IconButton
                        aria-label={`Remove ${user.code} from ${activeRole?.code}`}
                        onClick={() =>
                          runIfPermitted(session, "UserRoles.Delete", () => {
                            setDialogError("");
                            setRemoveUser(user);
                          })
                        }
                      >
                        <Delete />
                      </IconButton>
                    }
                  >
                    <ListItemText
                      primary={user.code}
                      secondary={`${user.name} · ${user.email}`}
                    />
                  </ListItem>
                ))}
              </List>
            )}
          </Paper>
        </Box>
      </Stack>
      <AddUsersDialog
        role={activeRole}
        open={addOpen}
        pending={pending}
        error={dialogError}
        onClose={() => setAddOpen(false)}
        onSave={assign}
      />
      <RemoveUserDialog
        user={removeUser}
        role={activeRole}
        pending={pending}
        error={dialogError}
        onClose={() => setRemoveUser(null)}
        onConfirm={remove}
      />
      <Snackbar
        open={Boolean(notice)}
        autoHideDuration={3000}
        message={notice}
        onClose={() => setNotice("")}
      />
    </Box>
  );
}
