import { useEffect, useMemo, useState } from "react";
import {
  Alert,
  Box,
  Button,
  Checkbox,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControlLabel,
  FormGroup,
  IconButton,
  MenuItem,
  Paper,
  Snackbar,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import AddCircleIcon from "@mui/icons-material/AddCircle";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import { runIfPermitted } from "../auth/permissions";
import {
  createRateLimitPolicy,
  deleteRateLimitPolicy,
  getRateLimitEndpoints,
  getRateLimitPolicies,
  updateRateLimitPolicy,
} from "./api/rateLimitsApi";

const dimensions = ["User", "IpAddress", "Application", "Endpoint"];
const empty = {
  applicationId: null,
  name: "",
  routePattern: "*",
  httpMethods: "",
  partitionBy: ["User", "Application", "Endpoint"],
  algorithm: "TokenBucket",
  permitLimit: 300,
  windowSeconds: 60,
  burstLimit: 30,
  priority: 0,
  isActive: true,
};

function PolicyDialog({
  open,
  policy,
  endpoints,
  pending,
  error,
  onClose,
  onSave,
}) {
  const [value, setValue] = useState(empty);
  useEffect(() => {
    if (!open) return;
    setValue(
      policy
        ? { ...policy, partitionBy: policy.partitionBy.split(",") }
        : empty,
    );
  }, [open, policy]);
  const set = (name) => (event) =>
    setValue((current) => ({ ...current, [name]: event.target.value }));
  const submit = () =>
    onSave({
      ...value,
      partitionBy: value.partitionBy.join(","),
      applicationId: value.applicationId || null,
      permitLimit: Number(value.permitLimit),
      windowSeconds: Number(value.windowSeconds),
      burstLimit: value.burstLimit === "" ? null : Number(value.burstLimit),
      priority: Number(value.priority),
      ...(policy ? { version: policy.version } : {}),
    });
  return (
    <Dialog
      open={open}
      onClose={pending ? undefined : onClose}
      fullWidth
      maxWidth="md"
    >
      <DialogTitle>
        {policy ? "Edit Rate Limit Policy" : "Create Rate Limit Policy"}
      </DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          {error && <Alert severity="error">{error}</Alert>}
          <TextField
            label="Name"
            required
            value={value.name}
            onChange={set("name")}
          />
          <TextField
            select
            label="API endpoint"
            value={`${value.httpMethods || "*"}|${value.routePattern}`}
            onChange={(event) => {
              const [method, ...route] = event.target.value.split("|");
              setValue((current) => ({
                ...current,
                httpMethods: method === "*" ? "" : method,
                routePattern: route.join("|"),
              }));
            }}
          >
            <MenuItem value="*|*">All endpoints</MenuItem>
            {endpoints.map((endpoint) => (
              <MenuItem
                key={`${endpoint.httpMethod}|${endpoint.route}`}
                value={`${endpoint.httpMethod}|${endpoint.route}`}
              >
                {endpoint.httpMethod} {endpoint.route}
              </MenuItem>
            ))}
          </TextField>
          <TextField
            select
            label="Algorithm"
            value={value.algorithm}
            onChange={set("algorithm")}
          >
            <MenuItem value="TokenBucket">Token Bucket</MenuItem>
            <MenuItem value="SlidingWindow">Sliding Window</MenuItem>
            <MenuItem value="FixedWindow">Fixed Window</MenuItem>
            <MenuItem value="Concurrency">Concurrency</MenuItem>
          </TextField>
          <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
            <TextField
              label="Permit limit"
              type="number"
              value={value.permitLimit}
              onChange={set("permitLimit")}
              fullWidth
            />
            <TextField
              label="Window (seconds)"
              type="number"
              value={value.windowSeconds}
              onChange={set("windowSeconds")}
              fullWidth
            />
            <TextField
              label="Burst limit"
              type="number"
              value={value.burstLimit ?? ""}
              onChange={set("burstLimit")}
              fullWidth
            />
            <TextField
              label="Priority"
              type="number"
              value={value.priority}
              onChange={set("priority")}
              fullWidth
            />
          </Stack>
          <Box>
            <Typography variant="subtitle2">Partition by</Typography>
            <FormGroup row>
              {dimensions.map((dimension) => (
                <FormControlLabel
                  key={dimension}
                  label={dimension}
                  control={
                    <Checkbox
                      checked={value.partitionBy.includes(dimension)}
                      onChange={(event) =>
                        setValue((current) => ({
                          ...current,
                          partitionBy: event.target.checked
                            ? [...current.partitionBy, dimension]
                            : current.partitionBy.filter(
                                (item) => item !== dimension,
                              ),
                        }))
                      }
                    />
                  }
                />
              ))}
            </FormGroup>
          </Box>
          <FormControlLabel
            label="Active"
            control={
              <Checkbox
                checked={value.isActive}
                onChange={(event) =>
                  setValue((current) => ({
                    ...current,
                    isActive: event.target.checked,
                  }))
                }
              />
            }
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={pending}>
          Cancel
        </Button>
        <Button
          variant="contained"
          onClick={submit}
          disabled={
            pending || !value.name.trim() || value.partitionBy.length === 0
          }
        >
          Save
        </Button>
      </DialogActions>
    </Dialog>
  );
}

export function RateLimitsPage({ session }) {
  const [items, setItems] = useState([]);
  const [endpoints, setEndpoints] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [editing, setEditing] = useState(undefined);
  const [open, setOpen] = useState(false);
  const [pending, setPending] = useState(false);
  const [dialogError, setDialogError] = useState("");
  const [notice, setNotice] = useState("");
  const [reload, setReload] = useState(0);
  useEffect(() => {
    const controller = new AbortController();
    setLoading(true);
    Promise.all([
      getRateLimitPolicies(controller.signal),
      getRateLimitEndpoints(controller.signal),
    ])
      .then(([policies, routes]) => {
        setItems(policies);
        setEndpoints(routes);
        setError("");
      })
      .catch(
        (reason) => reason.name !== "AbortError" && setError(reason.message),
      )
      .finally(() => setLoading(false));
    return () => controller.abort();
  }, [reload]);
  const sortedEndpoints = useMemo(
    () =>
      endpoints.filter((item) => item.route !== "api/rate-limiting/endpoints"),
    [endpoints],
  );
  const save = async (value) => {
    setPending(true);
    setDialogError("");
    try {
      if (editing) await updateRateLimitPolicy(editing.id, value);
      else await createRateLimitPolicy(value);
      setOpen(false);
      setNotice(editing ? "Policy updated." : "Policy created.");
      setReload((x) => x + 1);
    } catch (reason) {
      setDialogError(reason.message);
    } finally {
      setPending(false);
    }
  };
  const remove = async (item) => {
    if (!window.confirm(`Delete ${item.name}?`)) return;
    try {
      await deleteRateLimitPolicy(item.id, item.version);
      setNotice("Policy deleted.");
      setReload((x) => x + 1);
    } catch (reason) {
      setError(reason.message);
    }
  };
  return (
    <Box className="rate-limits-page">
      <Stack spacing={3} sx={{ height: "100%" }}>
        <Stack
          direction={{ xs: "column", sm: "row" }}
          gap={2}
          sx={{ justifyContent: "space-between" }}
        >
          <Box>
            <Typography component="h1" variant="h4" fontWeight={700}>
              Rate Limiting
            </Typography>
            <Typography color="text.secondary">
              Configure request limits by endpoint and caller dimensions.
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<AddCircleIcon />}
            onClick={() =>
              runIfPermitted(session, "RateLimiting.Create", () => {
                setEditing(undefined);
                setDialogError("");
                setOpen(true);
              })
            }
          >
            Create Policy
          </Button>
        </Stack>
        {error && <Alert severity="error">{error}</Alert>}
        <Paper className="rate-limits-table" variant="outlined">
          <TableContainer>
            <Table aria-label="Rate limit policies">
              <TableHead>
                <TableRow>
                  <TableCell>Name</TableCell>
                  <TableCell>Endpoint</TableCell>
                  <TableCell>Partition</TableCell>
                  <TableCell>Limit</TableCell>
                  <TableCell>Algorithm</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {items.map((item) => (
                  <TableRow key={item.id}>
                    <TableCell>{item.name}</TableCell>
                    <TableCell>
                      {item.httpMethods || "*"} {item.routePattern}
                    </TableCell>
                    <TableCell>{item.partitionBy}</TableCell>
                    <TableCell>
                      {item.permitLimit}/{item.windowSeconds}s
                      {item.burstLimit ? ` + ${item.burstLimit} burst` : ""}
                    </TableCell>
                    <TableCell>{item.algorithm}</TableCell>
                    <TableCell>
                      {item.isActive ? "Active" : "Inactive"}
                    </TableCell>
                    <TableCell align="right">
                      <IconButton
                        aria-label={`Edit ${item.name}`}
                        onClick={() =>
                          runIfPermitted(session, "RateLimiting.Update", () => {
                            setEditing(item);
                            setDialogError("");
                            setOpen(true);
                          })
                        }
                      >
                        <EditIcon />
                      </IconButton>
                      <IconButton
                        color="error"
                        aria-label={`Delete ${item.name}`}
                        onClick={() =>
                          runIfPermitted(session, "RateLimiting.Delete", () =>
                            remove(item),
                          )
                        }
                      >
                        <DeleteIcon />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
        {loading && <Typography>Loading...</Typography>}
      </Stack>
      <PolicyDialog
        open={open}
        policy={editing}
        endpoints={sortedEndpoints}
        pending={pending}
        error={dialogError}
        onClose={() => setOpen(false)}
        onSave={save}
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
