import { Button, Paper, Stack, TextField } from "@mui/material";

export function ActionsFilters({ value, onChange, onApply }) {
  const field = (name) => ({
    value: value[name],
    onChange: (event) => onChange({ ...value, [name]: event.target.value }),
  });
  return (
    <Paper variant="outlined" sx={{ p: 2 }}>
      <Stack direction={{ xs: "column", md: "row" }} spacing={2}>
        <TextField label="Code" size="small" {...field("code")} />
        <TextField label="Name" size="small" {...field("name")} />
        <Button variant="outlined" onClick={onApply}>
          Apply filters
        </Button>
      </Stack>
    </Paper>
  );
}
