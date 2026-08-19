import { Button, MenuItem, Paper, Stack, TextField } from "@mui/material";

export function ApplicationsFilters({ value, onChange, onApply }) {
  const field = (name) => ({
    value: value[name],
    onChange: (event) => onChange({ ...value, [name]: event.target.value }),
  });

  return (
    <Paper variant="outlined" sx={{ p: 2 }}>
      <Stack direction={{ xs: "column", md: "row" }} spacing={2}>
        <TextField label="Code" size="small" {...field("code")} />
        <TextField label="Name" size="small" {...field("name")} />
        <TextField label="Audience" size="small" {...field("audience")} />
        <TextField
          select
          label="Status"
          size="small"
          {...field("status")}
          sx={{ minWidth: 140 }}
        >
          <MenuItem value="">All statuses</MenuItem>
          <MenuItem value="active">Active</MenuItem>
          <MenuItem value="inactive">Inactive</MenuItem>
        </TextField>
        <Button variant="outlined" onClick={onApply}>
          Apply filters
        </Button>
      </Stack>
    </Paper>
  );
}
