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
  [1, "Application"],
  [2, "Code"],
  [3, "Name"],
  [4, "Resource Type"],
  [5, "Created"],
  [6, "Status"],
];

export function ResourcesTable({
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
        <Table aria-label="Resources">
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
            {items.map((resource) => (
              <TableRow key={resource.id} hover>
                <TableCell>
                  {resource.applicationCode} — {resource.applicationName}
                </TableCell>
                <TableCell>{resource.code}</TableCell>
                <TableCell>{resource.name}</TableCell>
                <TableCell>{resource.resourceType}</TableCell>
                <TableCell>
                  {new Date(resource.createdDate).toLocaleDateString()}
                </TableCell>
                <TableCell>
                  {resource.isActive ? "Active" : "Inactive"}
                </TableCell>
                {(canEdit || canDelete) && (
                  <TableCell align="right">
                    {canEdit && (
                      <IconButton
                        aria-label={`Edit ${resource.code}`}
                        onClick={() => onEdit(resource)}
                      >
                        <EditIcon />
                      </IconButton>
                    )}
                    {canDelete && (
                      <IconButton
                        aria-label={`Delete ${resource.code}`}
                        color="error"
                        onClick={() => onDelete(resource)}
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
