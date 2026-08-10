import {
  Button,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  TextField,
} from "@mui/material";

export function UsersFilters({ value, onChange, onApply }) {
  return (
    <Stack
      component="form"
      direction={{ xs: "column", md: "row" }}
      spacing={2}
      onSubmit={(event) => {
        event.preventDefault();
        onApply();
      }}
    >
      <TextField
        label="Code"
        value={value.code}
        onChange={(event) =>
          onChange({
            ...value,
            code: event.target.value,
          })
        }
      />
      <TextField
        label="Display name"
        value={value.name}
        onChange={(event) => onChange({ ...value, name: event.target.value })}
      />
      <FormControl sx={{ minWidth: 150 }}>
        <InputLabel id="user-status-label">Status</InputLabel>
        <Select
          labelId="user-status-label"
          label="Status"
          value={value.status}
          onChange={(event) =>
            onChange({ ...value, status: event.target.value })
          }
        >
          <MenuItem value="">All</MenuItem>
          <MenuItem value="active">Active</MenuItem>
          <MenuItem value="inactive">Inactive</MenuItem>
        </Select>
      </FormControl>
      <Button type="submit" variant="outlined">
        Apply filters
      </Button>
    </Stack>
  );
}
