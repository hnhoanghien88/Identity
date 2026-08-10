import {
  Alert,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from "@mui/material";

export function DeleteUserDialog({ user, pending, error, onClose, onConfirm }) {
  return (
    <Dialog open={Boolean(user)} onClose={pending ? undefined : onClose}>
      <DialogTitle>Delete user?</DialogTitle>
      <DialogContent>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}
        <DialogContentText>
          This will disable access for {user?.code} ({user?.name}).
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
          {pending ? <CircularProgress size={22} color="inherit" /> : "Delete"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
