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
  createApplication,
  deleteApplication,
  searchApplications,
  updateApplication,
} from "./api/applicationsApi";
import { ApplicationFormDialog } from "./components/ApplicationFormDialog";
import { ApplicationsFilters } from "./components/ApplicationsFilters";
import { ApplicationsTable } from "./components/ApplicationsTable";
import { DeleteApplicationDialog } from "./components/DeleteApplicationDialog";
import { runIfPermitted } from "../auth/permissions";
import { hasApplicationPermission } from "./capabilities";

const initialFilters = { code: "", name: "", audience: "", status: "" };

export function ApplicationsPage({ session }) {
  const canView = hasApplicationPermission(session, "Applications.Read");
  const [filters, setFilters] = useState(initialFilters);
  const [appliedFilters, setAppliedFilters] = useState(initialFilters);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [sort, setSort] = useState({ column: 4, direction: 1 });
  const [result, setResult] = useState({ items: [], totalCount: 0 });
  const [loaded, setLoaded] = useState(false);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState("");
  const [formApplication, setFormApplication] = useState(undefined);
  const [formOpen, setFormOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [pending, setPending] = useState(false);
  const [mutationError, setMutationError] = useState(null);
  const [notice, setNotice] = useState("");
  const [reloadKey, setReloadKey] = useState(0);

  const load = useCallback(
    (signal) => {
      if (!canView) return Promise.resolve();
      setLoading(true);
      setLoadError("");
      const filter = {
        ...(appliedFilters.code
          ? { code: { contains: appliedFilters.code } }
          : {}),
        ...(appliedFilters.name
          ? { name: { contains: appliedFilters.name } }
          : {}),
        ...(appliedFilters.audience
          ? { audience: { contains: appliedFilters.audience } }
          : {}),
        ...(appliedFilters.status
          ? { isActive: appliedFilters.status === "active" }
          : {}),
      };
      return searchApplications(
        { filter, sorts: [sort], page, pageSize },
        signal,
      )
        .then((data) => {
          setResult(data);
          setLoaded(true);
        })
        .catch((error) => {
          if (error.name !== "AbortError") setLoadError(error.message);
        })
        .finally(() => setLoading(false));
    },
    [canView, appliedFilters, sort, page, pageSize],
  );

  useEffect(() => {
    const controller = new AbortController();
    const timer = window.setTimeout(() => load(controller.signal), 0);
    return () => {
      window.clearTimeout(timer);
      controller.abort();
    };
  }, [load, reloadKey]);

  if (!canView) {
    return (
      <Alert severity="error">
        You do not have permission to view Applications.
      </Alert>
    );
  }

  const submitForm = async (values) => {
    setPending(true);
    setMutationError(null);
    try {
      if (formApplication) {
        await updateApplication(formApplication.id, values);
      } else {
        await createApplication(values);
      }
      setFormOpen(false);
      setNotice(
        formApplication ? "Application updated." : "Application created.",
      );
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
      await deleteApplication(deleteTarget.id, deleteTarget.version);
      setDeleteTarget(null);
      setNotice("Application deleted.");
      setReloadKey((key) => key + 1);
    } catch (error) {
      setMutationError(error);
    } finally {
      setPending(false);
    }
  };

  return (
    <Box className="applications-page">
      <Stack spacing={3}>
        <Stack
          direction={{ xs: "column", sm: "row" }}
          sx={{ justifyContent: "space-between" }}
          gap={2}
        >
          <Box>
            <Typography variant="h4" component="h1" fontWeight={700}>
              Applications
            </Typography>
            <Typography color="text.secondary">
              Manage registered identity applications.
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<AddCircleIcon />}
            onClick={() =>
              runIfPermitted(session, "Applications.Create", () => {
                setFormApplication(undefined);
                setMutationError(null);
                setFormOpen(true);
              })
            }
          >
            Create Application
          </Button>
        </Stack>
        <ApplicationsFilters
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
            <CircularProgress aria-label="Loading Applications" />
          </Box>
        ) : result.items.length === 0 ? (
          <Alert severity="info">
            No Applications match the current filters.
          </Alert>
        ) : (
          <ApplicationsTable
            {...result}
            page={page}
            pageSize={pageSize}
            sort={sort}
            canEdit
            canDelete
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
            onEdit={(application) =>
              runIfPermitted(session, "Applications.Update", () => {
                setFormApplication(application);
                setMutationError(null);
                setFormOpen(true);
              })
            }
            onDelete={(application) =>
              runIfPermitted(session, "Applications.Delete", () => {
                setDeleteTarget(application);
                setMutationError(null);
              })
            }
          />
        )}
      </Stack>
      <ApplicationFormDialog
        open={formOpen}
        application={formApplication}
        pending={pending}
        serverError={mutationError?.message}
        fieldErrors={mutationError?.errors}
        onClose={() => setFormOpen(false)}
        onSubmit={submitForm}
      />
      <DeleteApplicationDialog
        application={deleteTarget}
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
