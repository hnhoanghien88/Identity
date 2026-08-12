import {
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
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";

const columns = [
  [1, "Code"],
  [2, "Name"],
  [3, "Audience"],
  [4, "Created"],
  [5, "Status"],
];

export function ApplicationsTable({
  items,
  totalCount,
  page,
  pageSize,
  sort,
  canEdit,
  canDelete,
  onPage,
  onPageSize,
  onSort,
  onEdit,
  onDelete,
}) {
  return (
    <Paper variant="outlined">
      <TableContainer>
        <Table aria-label="Applications">
          <TableHead>
            <TableRow>
              {columns.map(([id, label]) => (
                <TableCell key={id}>
                  <TableSortLabel
                    active={sort.column === id}
                    direction={
                      sort.column === id && sort.direction === 1
                        ? "desc"
                        : "asc"
                    }
                    onClick={() => onSort(id)}
                  >
                    {label}
                  </TableSortLabel>
                </TableCell>
              ))}
              {(canEdit || canDelete) && (
                <TableCell align="right">Actions</TableCell>
              )}
            </TableRow>
          </TableHead>
          <TableBody>
            {items.map((application) => (
              <TableRow key={application.id} hover>
                <TableCell>{application.code}</TableCell>
                <TableCell>{application.name}</TableCell>
                <TableCell>{application.audience}</TableCell>
                <TableCell>
                  {new Date(application.createdDate).toLocaleDateString()}
                </TableCell>
                <TableCell>
                  {application.isActive ? "Active" : "Inactive"}
                </TableCell>
                {(canEdit || canDelete) && (
                  <TableCell align="right">
                    {canEdit && (
                      <IconButton
                        aria-label={`Edit ${application.code}`}
                        onClick={() => onEdit(application)}
                      >
                        <EditIcon />
                      </IconButton>
                    )}
                    {canDelete && (
                      <IconButton
                        aria-label={`Delete ${application.code}`}
                        color="error"
                        onClick={() => onDelete(application)}
                      >
                        <DeleteIcon />
                      </IconButton>
                    )}
                  </TableCell>
                )}
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
        rowsPerPageOptions={[10, 20, 50, 100]}
        onPageChange={(_, next) => onPage(next + 1)}
        onRowsPerPageChange={(event) => onPageSize(Number(event.target.value))}
      />
    </Paper>
  );
}
