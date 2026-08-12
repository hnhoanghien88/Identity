import { Button, MenuItem, Paper, Stack, TextField } from "@mui/material";

export function RolesFilters({ value, applications, onChange, onApply }) {
  return (
    <Paper
      component="form"
      onSubmit={(event) => {
        event.preventDefault();
        onApply();
      }}
      sx={{ p: 2 }}
    >
      <Stack direction={{ xs: "column", md: "row" }} spacing={2}>
        <TextField
          select
          label="Application"
          value={value.applicationId}
          onChange={(event) =>
            onChange({ ...value, applicationId: event.target.value })
          }
          sx={{ minWidth: 220 }}
        >
          <MenuItem value="">All Applications</MenuItem>
          {applications.map((application) => (
            <MenuItem key={application.id} value={String(application.id)}>
              {application.code} — {application.name}
            </MenuItem>
          ))}
        </TextField>
        <TextField
          label="Code"
          value={value.code}
          onChange={(event) => onChange({ ...value, code: event.target.value })}
        />
        <TextField
          label="Name"
          value={value.name}
          onChange={(event) => onChange({ ...value, name: event.target.value })}
        />
        <Button type="submit" variant="outlined">
          Apply filters
        </Button>
      </Stack>
    </Paper>
  );
}
