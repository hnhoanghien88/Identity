import { useCallback, useEffect, useMemo, useState } from "react";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Snackbar,
  Stack,
  Typography,
} from "@mui/material";
import AddCircleIcon from "@mui/icons-material/AddCircle";
import { hasPermission, runIfPermitted } from "../auth/permissions";
import {
  createUser,
  deleteUser,
  searchUsers,
  updateUser,
} from "./api/usersApi";
import { UsersFilters } from "./components/UsersFilters";
import { UsersTable } from "./components/UsersTable";
import { UserFormDialog } from "./components/UserFormDialog";
import { DeleteUserDialog } from "./components/DeleteUserDialog";

const initialFilters = {
  code: "",
  name: "",
  status: "",
};

export function UsersPage({ session }) {
  const canManage = hasPermission(session, "Users.Read");
  const currentUserId = useMemo(() => {
    try {
      return JSON.parse(
        atob(
          session.accessToken
            .split(".")[1]
            .replace(/-/g, "+")
            .replace(/_/g, "/"),
        ),
      ).uid;
    } catch {
      return null;
    }
  }, [session]);
  const [filters, setFilters] = useState(initialFilters);
  const [appliedFilters, setAppliedFilters] = useState(initialFilters);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [sort, setSort] = useState({ column: 3, direction: 1 });
  const [result, setResult] = useState({ items: [], totalCount: 0 });
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState("");
  const [formUser, setFormUser] = useState(undefined);
  const [formOpen, setFormOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [pending, setPending] = useState(false);
  const [mutationError, setMutationError] = useState(null);
  const [notice, setNotice] = useState("");
  const [reloadKey, setReloadKey] = useState(0);

  const load = useCallback(
    (signal) => {
      if (!canManage) return Promise.resolve();
      setLoading(true);
      setLoadError("");
      const filter = {
        ...(appliedFilters.code
          ? { code: { contains: appliedFilters.code } }
          : {}),
        ...(appliedFilters.name
          ? { name: { contains: appliedFilters.name } }
          : {}),
        ...(appliedFilters.status
          ? { isActive: appliedFilters.status === "active" }
          : {}),
      };
      return searchUsers({ filter, sorts: [sort], page, pageSize }, signal)
        .then((data) => setResult(data))
        .catch((error) => {
          if (error.name !== "AbortError") setLoadError(error.message);
        })
        .finally(() => setLoading(false));
    },
    [canManage, appliedFilters, sort, page, pageSize],
  );

  useEffect(() => {
    const controller = new AbortController();
    const loadTimer = window.setTimeout(() => {
      load(controller.signal);
    }, 0);

    return () => {
      window.clearTimeout(loadTimer);
      controller.abort();
    };
  }, [load, reloadKey]);
  if (!canManage)
    return (
      <Alert severity="error">
        You do not have permission to manage users.
      </Alert>
    );

  const submitForm = async (values) => {
    setPending(true);
    setMutationError(null);
    try {
      if (formUser) await updateUser(formUser.id, values);
      else await createUser(values);
      setFormOpen(false);
      setNotice(formUser ? "User updated." : "User created.");
      setReloadKey((key) => key + 1);
    } catch (error) {
      setMutationError(error);
    } finally {
      setPending(false);
    }
  };
  const confirmDelete = async () => {
    setPending(true);
    setMutationError(null);
    try {
      await deleteUser(deleteTarget.id, deleteTarget.version);
      setDeleteTarget(null);
      setNotice("User deleted.");
      setReloadKey((key) => key + 1);
    } catch (error) {
      setMutationError(error);
    } finally {
      setPending(false);
    }
  };
  return (
    <Box className="users-page">
      <Stack spacing={3}>
        <Stack
          direction={{ xs: "column", sm: "row" }}
          sx={{ justifyContent: "space-between" }}
          gap={2}
        >
          <Box>
            <Typography variant="h4" component="h1" fontWeight={700}>
              Users
            </Typography>
            <Typography color="text.secondary">
              Manage identity accounts.
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<AddCircleIcon />}
            onClick={() =>
              runIfPermitted(session, "Users.Create", () => {
                setFormUser(undefined);
                setMutationError(null);
                setFormOpen(true);
              })
            }
          >
            Create user
          </Button>
        </Stack>
        <UsersFilters
          value={filters}
          onChange={setFilters}
          onApply={() => {
            setPage(1);
            setAppliedFilters(filters);
          }}
        />
        {loadError && (
          <Alert
            severity="error"
            action={
              <Button
                color="inherit"
                onClick={() => setReloadKey((key) => key + 1)}
              >
                Retry
              </Button>
            }
          >
            {loadError}
          </Alert>
        )}
        {loading ? (
          <Box sx={{ display: "grid", placeItems: "center", py: 8 }}>
            <CircularProgress aria-label="Loading users" />
          </Box>
        ) : result.items.length === 0 ? (
          <Alert severity="info">No users match the current filters.</Alert>
        ) : (
          <UsersTable
            {...result}
            page={page}
            pageSize={pageSize}
            sort={sort}
            canEdit
            canDelete
            currentUserId={currentUserId}
            onPage={setPage}
            onPageSize={(size) => {
              setPageSize(size);
              setPage(1);
            }}
            onSort={(column) => {
              setSort((current) => ({
                column,
                direction:
                  current.column === column && current.direction === 0 ? 1 : 0,
              }));
              setPage(1);
            }}
            onEdit={(user) =>
              runIfPermitted(session, "Users.Update", () => {
                setFormUser(user);
                setMutationError(null);
                setFormOpen(true);
              })
            }
            onDelete={(user) =>
              runIfPermitted(session, "Users.Delete", () => {
                setDeleteTarget(user);
                setMutationError(null);
              })
            }
          />
        )}
      </Stack>
      <UserFormDialog
        open={formOpen}
        user={formUser}
        pending={pending}
        serverError={mutationError?.message}
        fieldErrors={mutationError?.errors}
        onClose={() => setFormOpen(false)}
        onSubmit={submitForm}
      />
      <DeleteUserDialog
        user={deleteTarget}
        pending={pending}
        error={mutationError?.message}
        onClose={() => setDeleteTarget(null)}
        onConfirm={confirmDelete}
      />
      <Snackbar
        open={Boolean(notice)}
        autoHideDuration={4000}
        onClose={() => setNotice("")}
        message={notice}
      />
    </Box>
  );
}
