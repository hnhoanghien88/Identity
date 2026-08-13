import { useCallback, useEffect, useRef, useState } from "react";
import { Alert, Box, Snackbar, Stack, Typography } from "@mui/material";
import { runIfPermitted } from "../auth/permissions";
import { searchRoles } from "../roles/api/rolesApi";
import { searchResources } from "../resources/api/resourcesApi";
import {
  getRolePermissions,
  grantRolePermission,
  revokeRolePermission,
} from "./api/rolePermissionsApi";
import { ActionsColumn } from "./components/ActionsColumn";
import { SelectionColumn } from "./components/SelectionColumn";

export function RolePermissionsPage({ session }) {
  const [roles, setRoles] = useState([]);
  const [resources, setResources] = useState([]);
  const [activeRoleId, setActiveRoleId] = useState(null);
  const [activeResourceId, setActiveResourceId] = useState(null);
  const [actions, setActions] = useState([]);
  const [lookupsLoading, setLookupsLoading] = useState(true);
  const [lookupError, setLookupError] = useState("");
  const [actionsLoading, setActionsLoading] = useState(false);
  const [actionsError, setActionsError] = useState("");
  const [pending, setPending] = useState(new Set());
  const [notice, setNotice] = useState("");
  const [reloadKey, setReloadKey] = useState(0);
  const requestGeneration = useRef(0);

  useEffect(() => {
    const controller = new AbortController();
    setLookupsLoading(true);
    setLookupError("");
    Promise.all([
      searchRoles(
        {
          filter: { isActive: true },
          sorts: [{ column: 2, direction: 0 }],
          page: 1,
          pageSize: 100,
        },
        controller.signal,
      ),
      searchResources(
        {
          filter: { isActive: true },
          sorts: [{ column: 1, direction: 0 }],
          page: 1,
          pageSize: 100,
        },
        controller.signal,
      ),
    ])
      .then(([roleResult, resourceResult]) => {
        setRoles(roleResult.items);
        setResources(resourceResult.items);
        setActiveRoleId(
          (current) => current ?? roleResult.items[0]?.id ?? null,
        );
        setActiveResourceId(
          (current) => current ?? resourceResult.items[0]?.id ?? null,
        );
      })
      .catch((error) => {
        if (error.name !== "AbortError") setLookupError(error.message);
      })
      .finally(() => setLookupsLoading(false));
    return () => controller.abort();
  }, []);

  const loadActions = useCallback(() => {
    if (!activeRoleId || !activeResourceId) {
      setActions([]);
      return () => {};
    }
    const controller = new AbortController();
    const generation = ++requestGeneration.current;
    setActionsLoading(true);
    setActionsError("");
    getRolePermissions(activeRoleId, activeResourceId, controller.signal)
      .then((snapshot) => {
        if (generation === requestGeneration.current)
          setActions(snapshot.actions);
      })
      .catch((error) => {
        if (
          error.name !== "AbortError" &&
          generation === requestGeneration.current
        )
          setActionsError(error.message);
      })
      .finally(() => {
        if (generation === requestGeneration.current) setActionsLoading(false);
      });
    return () => controller.abort();
  }, [activeRoleId, activeResourceId]);

  useEffect(() => loadActions(), [loadActions, reloadKey]);

  const toggle = async (actionId, granted) => {
    const previous = actions;
    setActions((current) =>
      current.map((action) =>
        action.actionId === actionId
          ? { ...action, isGranted: granted }
          : action,
      ),
    );
    setPending((current) => new Set(current).add(actionId));
    setActionsError("");
    try {
      if (granted)
        await grantRolePermission(activeRoleId, activeResourceId, actionId);
      else await revokeRolePermission(activeRoleId, activeResourceId, actionId);
      setNotice(granted ? "Permission granted." : "Permission revoked.");
    } catch (error) {
      setActions(previous);
      setActionsError(error.message);
    } finally {
      setPending((current) => {
        const next = new Set(current);
        next.delete(actionId);
        return next;
      });
    }
  };

  return (
    <Box className="role-permissions-page">
      <Stack spacing={3}>
        <Box>
          <Typography variant="h4" component="h1" fontWeight={700}>
            Role Permissions
          </Typography>
          <Typography color="text.secondary">
            Select a Role and Resource, then assign Actions.
          </Typography>
        </Box>
        {lookupError && <Alert severity="error">{lookupError}</Alert>}
        <Box className="role-permissions-grid">
          <SelectionColumn
            title="Roles"
            ariaLabel="Roles"
            items={roles}
            activeId={activeRoleId}
            loading={lookupsLoading}
            error=""
            emptyMessage="No Roles are available."
            primary={(role) => role.code}
            onSelect={setActiveRoleId}
          />
          <SelectionColumn
            title="Resources"
            ariaLabel="Resources"
            items={resources}
            activeId={activeResourceId}
            loading={lookupsLoading}
            error=""
            emptyMessage="No Resources are available."
            primary={(resource) => resource.resourceCode ?? resource.code}
            secondary={(resource) => resource.applicationCode}
            onSelect={setActiveResourceId}
          />
          <ActionsColumn
            actions={actions}
            loading={actionsLoading}
            error={actionsError}
            pending={pending}
            enabled={Boolean(activeRoleId && activeResourceId)}
            onToggle={(actionId, granted) =>
              runIfPermitted(
                session,
                granted ? "RolePermissions.Update" : "RolePermissions.Delete",
                () => toggle(actionId, granted),
              )
            }
            onRetry={() => setReloadKey((key) => key + 1)}
          />
        </Box>
      </Stack>
      <Snackbar
        open={Boolean(notice)}
        autoHideDuration={3000}
        message={notice}
        onClose={() => setNotice("")}
      />
    </Box>
  );
}
