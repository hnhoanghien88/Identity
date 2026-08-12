import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Typography,
} from "@mui/material";

export function DeleteApplicationDialog({
  application,
  pending,
  error,
  onClose,
  onConfirm,
}) {
  return (
    <Dialog open={Boolean(application)} onClose={pending ? undefined : onClose}>
      <DialogTitle>Delete Application?</DialogTitle>
      <DialogContent>
        {error && <Alert severity="error">{error}</Alert>}
        <Typography>
          Delete <strong>{application?.code}</strong> — {application?.name}?
          This action is blocked while related data exists.
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
