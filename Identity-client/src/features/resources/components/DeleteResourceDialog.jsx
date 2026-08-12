import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Typography,
} from "@mui/material";

export function DeleteResourceDialog({
  resource,
  pending,
  error,
  onClose,
  onConfirm,
}) {
  return (
    <Dialog open={Boolean(resource)} onClose={pending ? undefined : onClose}>
      <DialogTitle>Delete Resource?</DialogTitle>
      <DialogContent>
        {error && <Alert severity="error">{error}</Alert>}
        <Typography>
          Delete{" "}
          <strong>
            {resource?.applicationCode} / {resource?.code}
          </strong>{" "}
          — {resource?.name}? This action is blocked while Permissions or Menus
          reference the Resource.
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
