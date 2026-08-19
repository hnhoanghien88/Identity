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
  ["code", "Code", 1],
  ["name", "Display name", 2],
  ["createdDate", "Created", 3],
  ["isActive", "Status", 4],
];

export function UsersTable({
  items,
  totalCount,
  page,
  pageSize,
  sort,
  canEdit,
  canDelete,
  currentUserId,
  onPage,
  onPageSize,
  onSort,
  onEdit,
  onDelete,
}) {
  return (
    <Paper className="users-table" variant="outlined">
      <TableContainer>
        <Table size="small" aria-label="Users">
          <TableHead>
            <TableRow>
              {columns.map(([key, label, apiColumn]) => (
                <TableCell key={key}>
                  <TableSortLabel
                    active={sort.column === apiColumn}
                    direction={
                      sort.column === apiColumn && sort.direction === 1
                        ? "desc"
                        : "asc"
                    }
                    onClick={() => onSort(apiColumn)}
                  >
                    {label}
                  </TableSortLabel>
                </TableCell>
              ))}
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {items.map((user) => (
              <TableRow key={user.id} hover>
                <TableCell>{user.code}</TableCell>
                <TableCell>{user.name}</TableCell>
                <TableCell>
                  {new Date(user.createdDate).toLocaleString()}
                </TableCell>
                <TableCell>
                  <Chip
                    size="small"
                    color={user.isActive ? "success" : "default"}
                    label={user.isActive ? "Active" : "Inactive"}
                  />
                </TableCell>
                <TableCell align="right">
                  {canEdit && (
                    <Tooltip title="Edit user">
                      <IconButton
                        aria-label={`Edit ${user.code}`}
                        onClick={() => onEdit(user)}
                      >
                        <EditIcon />
                      </IconButton>
                    </Tooltip>
                  )}
                  {canDelete && String(user.id) !== String(currentUserId) && (
                    <Tooltip title="Delete user">
                      <IconButton
                        color="error"
                        aria-label={`Delete ${user.code}`}
                        onClick={() => onDelete(user)}
                      >
                        <DeleteIcon />
                      </IconButton>
                    </Tooltip>
                  )}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
        <TablePagination
          component="div"
          count={totalCount}
          page={page - 1}
          rowsPerPage={pageSize}
          onPageChange={(_, next) => onPage(next + 1)}
          onRowsPerPageChange={(event) =>
            onPageSize(Number(event.target.value))
          }
          rowsPerPageOptions={[10, 20, 50, 100]}
        />
      </TableContainer>
    </Paper>
  );
}
