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
import { searchResources } from "../resources/api/resourcesApi";
import {
  createMenu,
  deleteMenu,
  getMenuApplications,
  getMenus,
  updateMenu,
} from "./api/menusApi";
import { hasPermission, runIfPermitted } from "../auth/permissions";
import { getAuthorization } from "../auth/api/authorization";
import { publishSession } from "../auth/session";
import { DeleteMenuDialog } from "./components/DeleteMenuDialog";
import { MenuFormDialog } from "./components/MenuFormDialog";
import { MenusTreeTable } from "./components/MenusTreeTable";

const allIds = (nodes) =>
  nodes.flatMap((node) => [node.id, ...allIds(node.children || [])]);

const updateMenuInTree = (nodes, id, update) =>
  nodes.map((node) =>
    node.id === id
      ? {
          ...node,
          ...update,
          children: node.children,
        }
      : {
          ...node,
          children: updateMenuInTree(node.children || [], id, update),
        },
  );

export function MenusPage({ session }) {
  const canLoadResources = hasPermission(session, "Resources.Read");
  const canUpdateMenus = hasPermission(session, "Menus.Update");
  const [applications, setApplications] = useState([]);
  const [applicationId, setApplicationId] = useState("");
  const [menus, setMenus] = useState([]);
  const [resources, setResources] = useState([]);
  const [formMenus, setFormMenus] = useState([]);
  const [formApplicationId, setFormApplicationId] = useState("");
  const [lookupsLoading, setLookupsLoading] = useState(false);
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
  const [orderPendingIds, setOrderPendingIds] = useState(new Set());
  const refreshNavigation = () =>
    getAuthorization(session.accessToken)
      .then((authorization) => publishSession({ ...session, authorization }))
      .catch(() => undefined);
  useEffect(() => {
    const controller = new AbortController();
    getMenuApplications(controller.signal)
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
      try {
        const nextMenus = await getMenus(applicationId, signal);
        setMenus(nextMenus);
        if (expandedApplicationId.current !== applicationId) {
          setExpanded(new Set(allIds(nextMenus)));
          expandedApplicationId.current = applicationId;
        }
      } catch (reason) {
        if (reason.name !== "AbortError") setError(reason.message);
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
  useEffect(() => {
    if (!formOpen || !formApplicationId) {
      setFormMenus([]);
      setResources([]);
      return undefined;
    }
    const controller = new AbortController();
    setLookupsLoading(true);
    const resourcesRequest = canLoadResources
      ? searchResources(
          {
            filter: {
              applicationId: Number(formApplicationId),
              isActive: true,
            },
            sorts: [{ column: 2, direction: 0 }],
            page: 1,
            pageSize: 100,
          },
          controller.signal,
        )
      : Promise.resolve({ items: [] });
    Promise.all([
      getMenus(formApplicationId, controller.signal),
      resourcesRequest,
    ])
      .then(([nextMenus, resourcesResult]) => {
        setFormMenus(nextMenus);
        setResources(resourcesResult.items);
      })
      .catch((reason) => {
        if (reason.name !== "AbortError") setMutationError(reason);
      })
      .finally(() => {
        if (!controller.signal.aborted) setLookupsLoading(false);
      });
    return () => controller.abort();
  }, [canLoadResources, formApplicationId, formOpen]);
  const ids = useMemo(() => allIds(menus), [menus]);
  const submit = async (value) => {
    setPending(true);
    setMutationError(null);
    try {
      if (form) await updateMenu(form.id, value);
      else await createMenu(value);
      await refreshNavigation();
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
      await refreshNavigation();
      setDeleting(null);
      setNotice("Menu deleted.");
      setReload((key) => key + 1);
    } catch (reason) {
      setMutationError(reason);
    } finally {
      setPending(false);
    }
  };
  const updateSortOrder = async (menu, sortOrder) => {
    setOrderPendingIds((current) => new Set(current).add(menu.id));
    setError("");
    try {
      const updatedMenu = await updateMenu(menu.id, {
        applicationId: menu.applicationId,
        parentId: menu.parentId,
        resourceId: menu.resourceId,
        code: menu.code,
        name: menu.name,
        route: menu.route,
        icon: menu.icon,
        sortOrder,
        isVisible: menu.isVisible,
        isActive: menu.isActive,
        version: menu.version,
      });
      setMenus((current) =>
        updateMenuInTree(current, menu.id, {
          ...updatedMenu,
          sortOrder: updatedMenu?.sortOrder ?? sortOrder,
          version: updatedMenu?.version ?? menu.version + 1,
        }),
      );
      void refreshNavigation();
      setNotice(`Order for ${menu.name} updated.`);
    } catch (reason) {
      setError(reason.message);
    } finally {
      setOrderPendingIds((current) => {
        const next = new Set(current);
        next.delete(menu.id);
        return next;
      });
    }
  };
  return (
    <>
      <Stack className="menus-page" spacing={3}>
        <Stack
          direction={{ xs: "column", sm: "row" }}
          gap={2}
          sx={{ justifyContent: "space-between" }}
        >
          <Box>
            <Typography variant="h4" component="h1" fontWeight={700}>
              Menus
            </Typography>
            <Typography color="text.secondary">
              Manage recursive application navigation.
            </Typography>
          </Box>{" "}
          <Button
            variant="contained"
            startIcon={<AddCircleIcon />}
            disabled={!applicationId}
            onClick={() =>
              runIfPermitted(session, "Menus.Create", () => {
                setForm(null);
                setFormApplicationId(applicationId);
                setMutationError(null);
                setFormOpen(true);
              })
            }
          >
            Create Menu
          </Button>
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
            canEdit={canUpdateMenus}
            canDelete
            orderPendingIds={orderPendingIds}
            onOrderChange={updateSortOrder}
            onEdit={(value) =>
              runIfPermitted(session, "Menus.Update", () => {
                setForm(value);
                setFormApplicationId(String(value.applicationId));
                setMutationError(null);
                setFormOpen(true);
              })
            }
            onDelete={(value) =>
              runIfPermitted(session, "Menus.Delete", () => {
                setDeleting(value);
                setMutationError(null);
              })
            }
          />
        ) : (
          <Alert severity="info">No Menus exist for this Application.</Alert>
        )}
      </Stack>
      <MenuFormDialog
        open={formOpen}
        menu={form}
        applicationId={Number(applicationId)}
        applications={applications}
        menus={formMenus}
        resources={resources}
        lookupsLoading={lookupsLoading}
        pending={pending}
        serverError={mutationError?.message}
        fieldErrors={mutationError?.errors}
        onClose={() => setFormOpen(false)}
        onApplicationChange={(nextApplicationId) => {
          setFormApplicationId(nextApplicationId);
          setFormMenus([]);
          setResources([]);
          setMutationError(null);
        }}
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
    </>
  );
}
