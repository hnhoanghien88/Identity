import {
  Chip,
  IconButton,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TableSortLabel,
  Tooltip,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";

const columns = [
  [1, "Application"],
  [2, "Code"],
  [3, "Name"],
  [4, "System"],
  [5, "Active"],
  [6, "Created"],
];

export function RolesTable({
  items,
  totalCount,
  page,
  pageSize,
  sort,
  onPage,
  onPageSize,
  onSort,
  onEdit,
  onDelete,
}) {
  return (
    <Paper>
      <TableContainer>
        <Table aria-label="Roles">
          <TableHead>
            <TableRow>
              {columns.map(([column, label]) => (
                <TableCell key={column}>
                  <TableSortLabel
                    active={sort.column === column}
                    direction={sort.direction === 1 ? "desc" : "asc"}
                    onClick={() => onSort(column)}
                  >
                    {label}
                  </TableSortLabel>
                </TableCell>
              ))}
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {items.map((role) => (
              <TableRow key={role.id} hover>
                <TableCell>
                  {role.applicationCode} — {role.applicationName}
                </TableCell>
                <TableCell>{role.code}</TableCell>
                <TableCell>{role.name}</TableCell>
                <TableCell>
                  <Chip size="small" label={role.isSystemRole ? "Yes" : "No"} />
                </TableCell>
                <TableCell>
                  <Chip
                    size="small"
                    color={role.isActive ? "success" : "default"}
                    label={role.isActive ? "Active" : "Inactive"}
                  />
                </TableCell>
                <TableCell>
                  {new Date(role.createdDate).toLocaleString()}
                </TableCell>
                <TableCell align="right">
                  <Tooltip title={`Edit ${role.code}`}>
                    <IconButton
                      aria-label={`Edit ${role.code}`}
                      onClick={() => onEdit(role)}
                    >
                      <EditIcon />
                    </IconButton>
                  </Tooltip>
                  <Tooltip
                    title={
                      role.isSystemRole
                        ? "System Roles cannot be deleted"
                        : `Delete ${role.code}`
                    }
                  >
                    <span>
                      <IconButton
                        aria-label={`Delete ${role.code}`}
                        disabled={role.isSystemRole}
                        onClick={() => onDelete(role)}
                      >
                        <DeleteIcon />
                      </IconButton>
                    </span>
                  </Tooltip>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <TablePagination
        component="div"
        count={totalCount}
        page={page - 1}
        rowsPerPage={pageSize}
        onPageChange={(_, value) => onPage(value + 1)}
        onRowsPerPageChange={(event) => onPageSize(Number(event.target.value))}
        rowsPerPageOptions={[10, 20, 50, 100]}
      />
    </Paper>
  );
}
