import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  MenuItem,
  Snackbar,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import AddCircleIcon from "@mui/icons-material/AddCircle";
import UnfoldLessIcon from "@mui/icons-material/UnfoldLess";
import UnfoldMoreIcon from "@mui/icons-material/UnfoldMore";
import { searchApplications } from "../applications/api/applicationsApi";
import { searchResources } from "../resources/api/resourcesApi";
import { createMenu, deleteMenu, getMenus, updateMenu } from "./api/menusApi";
import { DeleteMenuDialog } from "./components/DeleteMenuDialog";
import { MenuFormDialog } from "./components/MenuFormDialog";
import { MenusTreeTable } from "./components/MenusTreeTable";

const allIds = (nodes) =>
  nodes.flatMap((node) => [node.id, ...allIds(node.children || [])]);

export function MenusPage() {
  const [applications, setApplications] = useState([]);
  const [applicationId, setApplicationId] = useState("");
  const [menus, setMenus] = useState([]);
  const [resources, setResources] = useState([]);
  const [expanded, setExpanded] = useState(new Set());
  const expandedApplicationId = useRef(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [reload, setReload] = useState(0);
  const [form, setForm] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [deleting, setDeleting] = useState(null);
  const [pending, setPending] = useState(false);
  const [mutationError, setMutationError] = useState(null);
  const [notice, setNotice] = useState("");
  useEffect(() => {
    const controller = new AbortController();
    searchApplications(
      {
        filter: { isActive: true },
        sorts: [{ column: 1, direction: 0 }],
        page: 1,
        pageSize: 100,
      },
      controller.signal,
    )
      .then((data) => {
        setApplications(data.items);
        setApplicationId(
          (current) => current || String(data.items[0]?.id || ""),
        );
      })
      .catch((value) => value.name !== "AbortError" && setError(value.message));
    return () => controller.abort();
  }, []);
  const load = useCallback(
    async (signal) => {
      if (!applicationId) {
        setLoading(false);
        return;
      }
      setLoading(true);
      setError("");
      const [menusResult, resourcesResult] = await Promise.allSettled([
        getMenus(applicationId, signal),
        searchResources(
          {
            filter: { applicationId: Number(applicationId), isActive: true },
            sorts: [{ column: 2, direction: 0 }],
            page: 1,
            pageSize: 100,
          },
          signal,
        ),
      ]);
      if (menusResult.status === "fulfilled") {
        const nextMenus = menusResult.value;
        setMenus(nextMenus);
        if (expandedApplicationId.current !== applicationId) {
          setExpanded(new Set(allIds(nextMenus)));
          expandedApplicationId.current = applicationId;
        }
      } else if (menusResult.reason.name !== "AbortError") {
        setError(menusResult.reason.message);
      }
      if (resourcesResult.status === "fulfilled") {
        setResources(resourcesResult.value.items);
      } else if (resourcesResult.reason.name !== "AbortError") {
        setResources([]);
        setError((current) =>
          current
            ? `${current} Resources: ${resourcesResult.reason.message}`
            : resourcesResult.reason.message,
        );
      }
      try {
      } finally {
        setLoading(false);
      }
    },
    [applicationId],
  );
  useEffect(() => {
    const controller = new AbortController();
    load(controller.signal);
    return () => controller.abort();
  }, [load, reload]);
  const ids = useMemo(() => allIds(menus), [menus]);
  const submit = async (value) => {
    setPending(true);
    setMutationError(null);
    try {
      if (form) await updateMenu(form.id, value);
      else await createMenu(value);
      setFormOpen(false);
      setNotice(form ? "Menu updated." : "Menu created.");
      setReload((key) => key + 1);
    } catch (reason) {
      setMutationError(reason);
    } finally {
      setPending(false);
    }
  };
  const confirmDelete = async () => {
    setPending(true);
    setMutationError(null);
    try {
      await deleteMenu(deleting.id, deleting.version);
      setDeleting(null);
      setNotice("Menu deleted.");
      setReload((key) => key + 1);
    } catch (reason) {
      setMutationError(reason);
    } finally {
      setPending(false);
    }
  };
  return (
    <Box>
      <Stack spacing={3}>
        <Stack
          direction={{ xs: "column", sm: "row" }}
          justifyContent="space-between"
          gap={2}
        >
          <Box>
            <Typography variant="h4" component="h1" fontWeight={700}>
              Menus
            </Typography>
            <Typography color="text.secondary">
              Manage recursive application navigation.
            </Typography>
          </Box>
          {
            <Button
              variant="contained"
              startIcon={<AddCircleIcon />}
              disabled={!applicationId}
              onClick={() => {
                setForm(null);
                setMutationError(null);
                setFormOpen(true);
              }}
            >
              Create Menu
            </Button>
          }
        </Stack>
        <Stack direction={{ xs: "column", sm: "row" }} gap={2}>
          <TextField
            select
            label="Application"
            value={applicationId}
            onChange={(event) => {
              setApplicationId(event.target.value);
              setExpanded(new Set());
            }}
            sx={{ minWidth: 280 }}
          >
            {applications.map((item) => (
              <MenuItem key={item.id} value={item.id}>
                {item.code} — {item.name}
              </MenuItem>
            ))}
          </TextField>
          <Button
            startIcon={<UnfoldMoreIcon />}
            onClick={() => setExpanded(new Set(ids))}
          >
            Expand all
          </Button>
          <Button
            startIcon={<UnfoldLessIcon />}
            onClick={() => setExpanded(new Set())}
          >
            Collapse all
          </Button>
        </Stack>
        {error && (
          <Alert
            severity="error"
            action={
              <Button
                color="inherit"
                onClick={() => setReload((key) => key + 1)}
              >
                Retry
              </Button>
            }
          >
            {error}
          </Alert>
        )}
        {loading ? (
          <Box sx={{ display: "grid", placeItems: "center", py: 8 }}>
            <CircularProgress aria-label="Loading Menus" />
          </Box>
        ) : menus.length ? (
          <MenusTreeTable
            menus={menus}
            expanded={expanded}
            onToggle={(id) =>
              setExpanded((current) => {
                const next = new Set(current);
                if (next.has(id)) next.delete(id);
                else next.add(id);
                return next;
              })
            }
            canEdit
            canDelete
            onEdit={(value) => {
              setForm(value);
              setMutationError(null);
              setFormOpen(true);
            }}
            onDelete={(value) => {
              setDeleting(value);
              setMutationError(null);
            }}
          />
        ) : (
          <Alert severity="info">No Menus exist for this Application.</Alert>
        )}
      </Stack>
      <MenuFormDialog
        open={formOpen}
        menu={form}
        applicationId={Number(applicationId)}
        menus={menus}
        resources={resources}
        pending={pending}
        serverError={mutationError?.message}
        fieldErrors={mutationError?.errors}
        onClose={() => setFormOpen(false)}
        onSubmit={submit}
      />
      <DeleteMenuDialog
        menu={deleting}
        pending={pending}
        error={mutationError?.message}
        onClose={() => setDeleting(null)}
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
