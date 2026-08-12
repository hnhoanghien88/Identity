import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Stack,
} from "@mui/material";

export function DeleteRoleDialog({ role, pending, error, onClose, onConfirm }) {
  return (
    <Dialog open={Boolean(role)} onClose={pending ? undefined : onClose}>
      <DialogTitle>Delete Role?</DialogTitle>
      <DialogContent>
        <Stack spacing={2}>
          {error && <Alert severity="error">{error}</Alert>}
          <DialogContentText>
            Delete <strong>{role?.code}</strong> ({role?.name}) from{" "}
            {role?.applicationName}? This cannot be undone.
          </DialogContentText>
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={pending}>
          Cancel
        </Button>
        <Button
          color="error"
          variant="contained"
          onClick={onConfirm}
          disabled={pending}
        >
          {pending ? "Deleting…" : "Delete"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
