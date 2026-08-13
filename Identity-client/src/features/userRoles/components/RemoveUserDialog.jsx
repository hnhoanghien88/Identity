import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from "@mui/material";
export function RemoveUserDialog({
  user,
  role,
  pending,
  error,
  onClose,
  onConfirm,
}) {
  return (
    <Dialog open={Boolean(user)} onClose={pending ? undefined : onClose}>
      <DialogTitle>Remove User from Role?</DialogTitle>
      <DialogContent>
        {error && <Alert severity="error">{error}</Alert>}
        <DialogContentText>
          Remove {user?.code} from {role?.code}? The User account will not be
          deleted.
        </DialogContentText>
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
          Remove
        </Button>
      </DialogActions>
    </Dialog>
  );
}
