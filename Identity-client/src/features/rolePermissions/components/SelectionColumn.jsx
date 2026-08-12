import {
  Alert,
  CircularProgress,
  List,
  ListItemButton,
  ListItemText,
  Paper,
  Stack,
  Typography,
} from "@mui/material";

export function SelectionColumn({
  title,
  ariaLabel,
  items,
  activeId,
  loading,
  error,
  emptyMessage,
  primary,
  secondary,
  onSelect,
}) {
  return (
    <Paper className="role-permissions-column" variant="outlined">
      <Stack spacing={1.5}>
        <Typography component="h2" variant="h6" fontWeight={700}>
          {title}
        </Typography>
        {error && <Alert severity="error">{error}</Alert>}
        {loading && items.length === 0 ? (
          <CircularProgress size={28} aria-label={`Loading ${title}`} />
        ) : items.length === 0 ? (
          <Alert severity="info">{emptyMessage}</Alert>
        ) : (
          <List aria-label={ariaLabel} disablePadding>
            {items.map((item) => (
              <ListItemButton
                key={item.id}
                selected={item.id === activeId}
                aria-current={item.id === activeId ? "true" : undefined}
                onClick={() => onSelect(item.id)}
              >
                <ListItemText
                  primary={primary(item)}
                  secondary={secondary?.(item)}
                />
              </ListItemButton>
            ))}
          </List>
        )}
      </Stack>
    </Paper>
  );
}
