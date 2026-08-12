import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Typography,
} from "@mui/material";

export function DeleteActionDialog({
  action,
  pending,
  error,
  onClose,
  onConfirm,
}) {
  return (
    <Dialog open={Boolean(action)} onClose={pending ? undefined : onClose}>
      <DialogTitle>Delete Action?</DialogTitle>
      <DialogContent>
        {error && <Alert severity="error">{error}</Alert>}
        <Typography>
          Delete <strong>{action?.code}</strong> — {action?.name}? This action
          is blocked while related Permissions exist.
        </Typography>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={pending}>
          Cancel
        </Button>
        <Button
          onClick={onConfirm}
          disabled={pending}
          color="error"
          variant="contained"
        >
          {pending ? "Deleting…" : "Delete"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
