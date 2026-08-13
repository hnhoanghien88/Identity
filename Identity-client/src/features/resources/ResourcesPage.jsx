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
  createResource,
  deleteResource,
  searchResources,
  updateResource,
} from "./api/resourcesApi";
import { searchApplications } from "../applications/api/applicationsApi";
import { hasPermission, runIfPermitted } from "../auth/permissions";
import { hasResourcePermission } from "./capabilities";
import { DeleteResourceDialog } from "./components/DeleteResourceDialog";
import { ResourceFormDialog } from "./components/ResourceFormDialog";
import { ResourcesFilters } from "./components/ResourcesFilters";
import { ResourcesTable } from "./components/ResourcesTable";

const initialFilters = {
  applicationId: "",
  code: "",
  name: "",
  resourceType: "",
  status: "",
};

export function ResourcesPage({ session }) {
  const canView = hasResourcePermission(session, "Resources.Read");
  const canViewApplications = hasPermission(session, "Applications.Read");
  const [applications, setApplications] = useState([]);
  const [filters, setFilters] = useState(initialFilters);
  const [appliedFilters, setAppliedFilters] = useState(initialFilters);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [sort, setSort] = useState({
    column: 5,
    direction: 1,
  });
  const [result, setResult] = useState({
    items: [],
    totalCount: 0,
  });
  const [loaded, setLoaded] = useState(false);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState("");
  const [formResource, setFormResource] = useState(undefined);
  const [formOpen, setFormOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [pending, setPending] = useState(false);
  const [mutationError, setMutationError] = useState(null);
  const [notice, setNotice] = useState("");
  const [reloadKey, setReloadKey] = useState(0);

  const load = useCallback(
    async (signal) => {
      if (!canView) return;
      setLoading(true);
      setLoadError("");
      const filter = {
        ...(appliedFilters.applicationId
          ? {
              applicationId: Number(appliedFilters.applicationId),
            }
          : {}),
        ...(appliedFilters.code
          ? {
              code: {
                contains: appliedFilters.code,
              },
            }
          : {}),
        ...(appliedFilters.name
          ? {
              name: {
                contains: appliedFilters.name,
              },
            }
          : {}),
        ...(appliedFilters.resourceType
          ? {
              resourceType: {
                contains: appliedFilters.resourceType,
              },
            }
          : {}),
        ...(appliedFilters.status
          ? {
              isActive: appliedFilters.status === "active",
            }
          : {}),
      };

      try {
        const resourceData = await searchResources(
          {
            filter,
            sorts: [sort],
            page,
            pageSize,
          },
          signal,
        );
        const applicationItems = canViewApplications
          ? (
              await searchApplications(
                {
                  filter: { isActive: true },
                  sorts: [{ column: 1, direction: 0 }],
                  page: 1,
                  pageSize: 100,
                },
                signal,
              )
            ).items
          : Array.from(
              new Map(
                resourceData.items.map((item) => [
                  item.applicationId,
                  {
                    id: item.applicationId,
                    code: item.applicationCode,
                    name: item.applicationName,
                  },
                ]),
              ).values(),
            );
        setResult(resourceData);
        setApplications(applicationItems);
        setLoaded(true);
      } catch (error) {
        if (error.name !== "AbortError") setLoadError(error.message);
      } finally {
        setLoading(false);
      }
    },
    [canView, canViewApplications, appliedFilters, sort, page, pageSize],
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
        You do not have permission to view Resources.
      </Alert>
    );
  }

  const submitForm = async (values) => {
    setPending(true);
    setMutationError(null);
    try {
      if (formResource) {
        await updateResource(formResource.id, values);
      } else {
        await createResource(values);
      }
      setFormOpen(false);
      setNotice(formResource ? "Resource updated." : "Resource created.");
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
      await deleteResource(deleteTarget.id, deleteTarget.version);
      setDeleteTarget(null);
      setNotice("Resource deleted.");
      setReloadKey((key) => key + 1);
    } catch (error) {
      setMutationError(error);
    } finally {
      setPending(false);
    }
  };

  return (
    <Box className="resources-page">
      <Stack spacing={3}>
        <Stack
          direction={{
            xs: "column",
            sm: "row",
          }}
          sx={{
            justifyContent: "space-between",
          }}
          gap={2}
        >
          <Box>
            <Typography variant="h4" component="h1" fontWeight={700}>
              Resources
            </Typography>
            <Typography color="text.secondary">
              Manage protected resources for identity applications.
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<AddCircleIcon />}
            onClick={() =>
              runIfPermitted(session, "Resources.Create", () => {
                setFormResource(undefined);
                setMutationError(null);
                setFormOpen(true);
              })
            }
          >
            Create Resource
          </Button>
        </Stack>
        <ResourcesFilters
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
          <Box
            sx={{
              display: "grid",
              placeItems: "center",
              py: 8,
            }}
          >
            <CircularProgress aria-label="Loading Resources" />
          </Box>
        ) : result.items.length === 0 ? (
          <Alert severity="info">No Resources match the current filters.</Alert>
        ) : (
          <ResourcesTable
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
            onEdit={(resource) =>
              runIfPermitted(session, "Resources.Update", () => {
                setFormResource(resource);
                setMutationError(null);
                setFormOpen(true);
              })
            }
            onDelete={(resource) =>
              runIfPermitted(session, "Resources.Delete", () => {
                setDeleteTarget(resource);
                setMutationError(null);
              })
            }
          />
        )}
      </Stack>
      <ResourceFormDialog
        open={formOpen}
        resource={formResource}
        applications={applications}
        pending={pending}
        serverError={mutationError?.message}
        fieldErrors={mutationError?.errors}
        onClose={() => setFormOpen(false)}
        onSubmit={submitForm}
      />
      <DeleteResourceDialog
        resource={deleteTarget}
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
