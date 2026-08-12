import { Fragment } from "react";
import {
  IconButton,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tooltip,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import KeyboardArrowRightIcon from "@mui/icons-material/KeyboardArrowRight";

const flattenMenus = (nodes, expanded, depth = 0) =>
  nodes.flatMap((node) => [
    { node, depth },
    ...(expanded.has(node.id)
      ? flattenMenus(node.children || [], expanded, depth + 1)
      : []),
  ]);

export function MenusTreeTable({
  menus,
  expanded,
  onToggle,
  canEdit,
  canDelete,
  onEdit,
  onDelete,
}) {
  return (
    <TableContainer component={Paper} variant="outlined">
      <Table aria-label="Menus tree">
        <TableHead>
          <TableRow>
            <TableCell>Name</TableCell>
            <TableCell>Code</TableCell>
            <TableCell>Resource</TableCell>
            <TableCell>Route</TableCell>
            <TableCell>Order</TableCell>
            <TableCell>Status</TableCell>
            <TableCell align="right">Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {flattenMenus(menus, expanded).map(({ node, depth }) => {
            const hasChildren = Boolean(node.children?.length);
            return (
              <Fragment key={node.id}>
                <TableRow>
                  <TableCell>
                    <Stack
                      direction="row"
                      alignItems="center"
                      sx={{ pl: depth * 3 }}
                    >
                      <IconButton
                        size="small"
                        disabled={!hasChildren}
                        aria-label={`${expanded.has(node.id) ? "Collapse" : "Expand"} ${node.name}`}
                        aria-expanded={
                          hasChildren ? expanded.has(node.id) : undefined
                        }
                        onClick={() => onToggle(node.id)}
                      >
                        {expanded.has(node.id) ? (
                          <KeyboardArrowDownIcon />
                        ) : (
                          <KeyboardArrowRightIcon />
                        )}
                      </IconButton>
                      {node.name}
                    </Stack>
                  </TableCell>
                  <TableCell>{node.code}</TableCell>
                  <TableCell>{node.resourceName || "â€”"}</TableCell>
                  <TableCell>{node.route || "â€”"}</TableCell>
                  <TableCell>{node.sortOrder}</TableCell>
                  <TableCell>
                    {node.isActive
                      ? node.isVisible
                        ? "Visible"
                        : "Hidden"
                      : "Inactive"}
                  </TableCell>
                  <TableCell align="right">
                    {canEdit && (
                      <Tooltip title="Edit">
                        <IconButton
                          aria-label={`Edit ${node.name}`}
                          onClick={() => onEdit(node)}
                        >
                          <EditIcon />
                        </IconButton>
                      </Tooltip>
                    )}
                    {canDelete && (
                      <Tooltip title="Delete">
                        <IconButton
                          aria-label={`Delete ${node.name}`}
                          onClick={() => onDelete(node)}
                        >
                          <DeleteIcon />
                        </IconButton>
                      </Tooltip>
                    )}
                  </TableCell>
                </TableRow>
              </Fragment>
            );
          })}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
