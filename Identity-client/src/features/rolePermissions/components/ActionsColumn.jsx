import {
  Alert,
  Button,
  Checkbox,
  CircularProgress,
  FormControlLabel,
  Paper,
  Stack,
  Typography,
} from "@mui/material";

export function ActionsColumn({
  actions,
  loading,
  error,
  pending,
  enabled,
  onToggle,
  onRetry,
}) {
  return (
    <Paper className="role-permissions-column" variant="outlined">
      <Stack spacing={1.5}>
        <Typography component="h2" variant="h6" fontWeight={700}>
          Actions
        </Typography>
        {error && (
          <Alert
            severity="error"
            action={
              onRetry && (
                <Button color="inherit" onClick={onRetry}>
                  Retry
                </Button>
              )
            }
          >
            {error}
          </Alert>
        )}
        {loading && actions.length === 0 ? (
          <CircularProgress size={28} aria-label="Loading Actions" />
        ) : !enabled ? (
          <Alert severity="info">Select an available Role and Resource.</Alert>
        ) : actions.length === 0 ? (
          <Alert severity="info">No Actions are available.</Alert>
        ) : (
          <Stack role="group" aria-label="Permission Actions">
            {actions.map((action) => (
              <FormControlLabel
                key={action.actionId}
                control={
                  <Checkbox
                    checked={action.isGranted}
                    disabled={pending.has(action.actionId)}
                    onChange={(event) =>
                      onToggle(action.actionId, event.target.checked)
                    }
                  />
                }
                label={`${action.code} — ${action.name}`}
              />
            ))}
          </Stack>
        )}
      </Stack>
    </Paper>
  );
}
