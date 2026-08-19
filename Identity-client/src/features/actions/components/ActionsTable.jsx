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
  [3, "Created"],
];

export function ActionsTable({
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
    <Paper className="actions-table" variant="outlined">
      <TableContainer>
        <Table aria-label="Actions">
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
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {items.map((action) => (
              <TableRow key={action.id} hover>
                <TableCell>{action.code}</TableCell>
                <TableCell>{action.name}</TableCell>
                <TableCell>
                  {new Date(action.createdDate).toLocaleDateString()}
                </TableCell>
                <TableCell align="right">
                  <IconButton
                    aria-label={`Edit ${action.code}`}
                    onClick={() => onEdit(action)}
                  >
                    <EditIcon />
                  </IconButton>
                  <IconButton
                    aria-label={`Delete ${action.code}`}
                    color="error"
                    onClick={() => onDelete(action)}
                  >
                    <DeleteIcon />
                  </IconButton>
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
        rowsPerPageOptions={[10, 20, 50, 100]}
        onPageChange={(_, next) => onPage(next + 1)}
        onRowsPerPageChange={(event) => onPageSize(Number(event.target.value))}
      />
    </Paper>
  );
}
