import { Button, MenuItem, Paper, Stack, TextField } from "@mui/material";

export function ResourcesFilters({ value, applications, onChange, onApply }) {
  const field = (name) => ({
    value: value[name],
    onChange: (event) =>
      onChange({
        ...value,
        [name]: event.target.value,
      }),
  });

  return (
    <Paper variant="outlined" sx={{ p: 2 }}>
      <Stack direction={{ xs: "column", lg: "row" }} spacing={2}>
        <TextField
          select
          label="Application"
          size="small"
          {...field("applicationId")}
          sx={{ minWidth: 180 }}
        >
          <MenuItem value="">All Applications</MenuItem>
          {applications.map((application) => (
            <MenuItem key={application.id} value={application.id}>
              {application.code} — {application.name}
            </MenuItem>
          ))}
        </TextField>
        <TextField label="Code" size="small" {...field("code")} />
        <TextField label="Name" size="small" {...field("name")} />
        <TextField
          label="Resource Type"
          size="small"
          {...field("resourceType")}
        />
        <TextField select label="Status" size="small" {...field("status")}>
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
