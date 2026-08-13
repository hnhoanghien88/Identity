import { useCallback, useEffect, useState } from "react";
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
import { runIfPermitted } from "../auth/permissions";
import {
  createRole,
  getRoleApplications,
  deleteRole,
  searchRoles,
  updateRole,
} from "./api/rolesApi";
import { DeleteRoleDialog } from "./components/DeleteRoleDialog";
import { RoleFormDialog } from "./components/RoleFormDialog";
import { RolesFilters } from "./components/RolesFilters";
import { RolesTable } from "./components/RolesTable";

const initialFilters = { applicationId: "", code: "", name: "" };

export function RolesPage({ session }) {
  const [applications, setApplications] = useState([]);
  const [filters, setFilters] = useState(initialFilters);
  const [appliedFilters, setAppliedFilters] = useState(initialFilters);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [sort, setSort] = useState({ column: 6, direction: 1 });
  const [result, setResult] = useState({ items: [], totalCount: 0 });
  const [loaded, setLoaded] = useState(false);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState("");
  const [formRole, setFormRole] = useState(undefined);
  const [formOpen, setFormOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [pending, setPending] = useState(false);
  const [mutationError, setMutationError] = useState(null);
  const [notice, setNotice] = useState("");
  const [reloadKey, setReloadKey] = useState(0);

  useEffect(() => {
    getRoleApplications()
      .then((data) => setApplications(data.items))
      .catch(() => setApplications([]));
  }, []);

  const load = useCallback(
    (signal) => {
      setLoading(true);
      setLoadError("");
      const filter = {
        ...(appliedFilters.applicationId
          ? { applicationIds: [Number(appliedFilters.applicationId)] }
          : {}),
        ...(appliedFilters.code
          ? { code: { contains: appliedFilters.code } }
          : {}),
        ...(appliedFilters.name
          ? { name: { contains: appliedFilters.name } }
          : {}),
      };
      return searchRoles({ filter, sorts: [sort], page, pageSize }, signal)
        .then((data) => {
          setResult(data);
          setLoaded(true);
        })
        .catch((error) => {
          if (error.name !== "AbortError") setLoadError(error.message);
        })
        .finally(() => setLoading(false));
    },
    [appliedFilters, sort, page, pageSize],
  );

  useEffect(() => {
    const controller = new AbortController();
    const timer = window.setTimeout(() => load(controller.signal), 0);
    return () => {
      window.clearTimeout(timer);
      controller.abort();
    };
  }, [load, reloadKey]);

  const submitForm = async (values) => {
    setPending(true);
    setMutationError(null);
    try {
      if (formRole) await updateRole(formRole.id, values);
      else await createRole(values);
      setFormOpen(false);
      setNotice(formRole ? "Role updated." : "Role created.");
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
      await deleteRole(deleteTarget.id, deleteTarget.version);
      setDeleteTarget(null);
      setNotice("Role deleted.");
      setReloadKey((key) => key + 1);
    } catch (error) {
      setMutationError(error);
    } finally {
      setPending(false);
    }
  };

  return (
    <Box className="roles-page">
      <Stack spacing={3}>
        <Stack
          direction={{ xs: "column", sm: "row" }}
          sx={{ justifyContent: "space-between" }}
          gap={2}
        >
          <Box>
            <Typography variant="h4" component="h1" fontWeight={700}>
              Roles
            </Typography>
            <Typography color="text.secondary">
              Manage application Roles.
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<AddCircleIcon />}
            onClick={() =>
              runIfPermitted(session, "Roles.Create", () => {
                setFormRole(undefined);
                setMutationError(null);
                setFormOpen(true);
              })
            }
          >
            Create Role
          </Button>
        </Stack>
        <RolesFilters
          value={filters}
          applications={applications}
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
        {loading && !loaded ? (
          <Box sx={{ display: "grid", placeItems: "center", py: 8 }}>
            <CircularProgress aria-label="Loading Roles" />
          </Box>
        ) : result.items.length === 0 ? (
          <Alert severity="info">No Roles match the current filters.</Alert>
        ) : (
          <RolesTable
            {...result}
            page={page}
            pageSize={pageSize}
            sort={sort}
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
            onEdit={(role) =>
              runIfPermitted(session, "Roles.Update", () => {
                setFormRole(role);
                setMutationError(null);
                setFormOpen(true);
              })
            }
            onDelete={(role) =>
              runIfPermitted(session, "Roles.Delete", () => {
                setDeleteTarget(role);
                setMutationError(null);
              })
            }
          />
        )}
      </Stack>
      <RoleFormDialog
        open={formOpen}
        role={formRole}
        applications={applications}
        pending={pending}
        serverError={mutationError?.message}
        fieldErrors={mutationError?.errors}
        onClose={() => setFormOpen(false)}
        onSubmit={submitForm}
      />
      <DeleteRoleDialog
        role={deleteTarget}
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
