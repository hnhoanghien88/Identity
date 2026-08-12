import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Typography,
} from "@mui/material";

export function DeleteMenuDialog({ menu, pending, error, onClose, onConfirm }) {
  return (
    <Dialog open={Boolean(menu)} onClose={pending ? undefined : onClose}>
      <DialogTitle>Delete Menu</DialogTitle>
      <DialogContent>
        {error && <Alert severity="error">{error}</Alert>}
        <Typography>
          Delete{" "}
          <strong>
            {menu?.code} — {menu?.name}
          </strong>
          ?
        </Typography>
        <Typography color="text.secondary">
          Direct children: {menu?.children?.length || 0}. Menus with children
          cannot be deleted.
        </Typography>
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
          Delete
        </Button>
      </DialogActions>
    </Dialog>
  );
}
