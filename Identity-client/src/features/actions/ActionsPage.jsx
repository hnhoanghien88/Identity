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
import {
  createAction,
  deleteAction,
  searchActions,
  updateAction,
} from "./api/actionsApi";
import { ActionFormDialog } from "./components/ActionFormDialog";
import { ActionsFilters } from "./components/ActionsFilters";
import { ActionsTable } from "./components/ActionsTable";
import { DeleteActionDialog } from "./components/DeleteActionDialog";

const initialFilters = { code: "", name: "" };

export function ActionsPage() {
  const [filters, setFilters] = useState(initialFilters);
  const [appliedFilters, setAppliedFilters] = useState(initialFilters);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [sort, setSort] = useState({ column: 3, direction: 1 });
  const [result, setResult] = useState({ items: [], totalCount: 0 });
  const [loaded, setLoaded] = useState(false);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState("");
  const [formAction, setFormAction] = useState(undefined);
  const [formOpen, setFormOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [pending, setPending] = useState(false);
  const [mutationError, setMutationError] = useState(null);
  const [notice, setNotice] = useState("");
  const [reloadKey, setReloadKey] = useState(0);

  const load = useCallback(
    (signal) => {
      setLoading(true);
      setLoadError("");
      const filter = {
        ...(appliedFilters.code
          ? { code: { contains: appliedFilters.code } }
          : {}),
        ...(appliedFilters.name
          ? { name: { contains: appliedFilters.name } }
          : {}),
      };
      return searchActions({ filter, sorts: [sort], page, pageSize }, signal)
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
      if (formAction) await updateAction(formAction.id, values);
      else await createAction(values);
      setFormOpen(false);
      setNotice(formAction ? "Action updated." : "Action created.");
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
      await deleteAction(deleteTarget.id, deleteTarget.version);
      setDeleteTarget(null);
      setNotice("Action deleted.");
      setReloadKey((key) => key + 1);
    } catch (error) {
      setMutationError(error);
    } finally {
      setPending(false);
    }
  };

  return (
    <Box className="actions-page">
      <Stack spacing={3}>
        <Stack
          direction={{ xs: "column", sm: "row" }}
          sx={{ justifyContent: "space-between" }}
          gap={2}
        >
          <Box>
            <Typography variant="h4" component="h1" fontWeight={700}>
              Actions
            </Typography>
            <Typography color="text.secondary">
              Manage actions used by Permissions.
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<AddCircleIcon />}
            onClick={() => {
              setFormAction(undefined);
              setMutationError(null);
              setFormOpen(true);
            }}
          >
            Create Action
          </Button>
        </Stack>
        <ActionsFilters
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
        {loading && !loaded ? (
          <Box sx={{ display: "grid", placeItems: "center", py: 8 }}>
            <CircularProgress aria-label="Loading Actions" />
          </Box>
        ) : result.items.length === 0 ? (
          <Alert severity="info">No Actions match the current filters.</Alert>
        ) : (
          <ActionsTable
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
            onEdit={(action) => {
              setFormAction(action);
              setMutationError(null);
              setFormOpen(true);
            }}
            onDelete={(action) => {
              setDeleteTarget(action);
              setMutationError(null);
            }}
          />
        )}
      </Stack>
      <ActionFormDialog
        open={formOpen}
        action={formAction}
        pending={pending}
        serverError={mutationError?.message}
        fieldErrors={mutationError?.errors}
        onClose={() => setFormOpen(false)}
        onSubmit={submitForm}
      />
      <DeleteActionDialog
        action={deleteTarget}
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
