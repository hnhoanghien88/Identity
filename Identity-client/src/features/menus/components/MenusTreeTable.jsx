import { Fragment, useEffect, useState } from "react";
import {
  CircularProgress,
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
  TextField,
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

function InlineOrderCell({ menu, canEdit, pending, onChange }) {
  const [value, setValue] = useState(String(menu.sortOrder));

  useEffect(() => {
    setValue(String(menu.sortOrder));
  }, [menu.sortOrder]);

  if (!canEdit) return menu.sortOrder;

  const save = (candidate = value) => {
    const next = Number(candidate);
    if (!Number.isInteger(next)) {
      setValue(String(menu.sortOrder));
      return;
    }
    if (next !== menu.sortOrder) onChange(menu, next);
  };

  return (
    <TextField
      size="small"
      type="number"
      value={value}
      disabled={pending}
      onChange={(event) => setValue(event.target.value)}
      onBlur={save}
      onKeyDown={(event) => {
        if (event.key === "Enter") {
          event.preventDefault();
          save(event.currentTarget.value);
        }
        if (event.key === "Escape") {
          setValue(String(menu.sortOrder));
        }
      }}
      slotProps={{
        htmlInput: {
          "aria-label": `Order for ${menu.name}`,
          step: 1,
        },
        input: {
          endAdornment: pending ? <CircularProgress size={16} /> : null,
        },
      }}
      sx={{ width: 100 }}
    />
  );
}

export function MenusTreeTable({
  menus,
  expanded,
  onToggle,
  canEdit,
  canDelete,
  onEdit,
  onDelete,
  orderPendingIds = new Set(),
  onOrderChange,
}) {
  return (
    <TableContainer
      className="menus-tree-table"
      component={Paper}
      variant="outlined"
    >
      <Table aria-label="Menus tree">
        <TableHead>
          <TableRow>
            <TableCell>Name</TableCell>
            <TableCell>Code</TableCell>
            <TableCell>Resource</TableCell>
            <TableCell>Route</TableCell>
            <TableCell>Order</TableCell>
            <TableCell>IsVisible</TableCell>
            <TableCell>IsActive</TableCell>
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
                      sx={{ alignItems: "center", pl: depth * 3 }}
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
                  <TableCell>{node.resourceName || "—"}</TableCell>
                  <TableCell>{node.route || "—"}</TableCell>
                  <TableCell>
                    <InlineOrderCell
                      menu={node}
                      canEdit={canEdit && Boolean(onOrderChange)}
                      pending={orderPendingIds.has(node.id)}
                      onChange={onOrderChange}
                    />
                  </TableCell>
                  <TableCell>{node.isVisible ? "Hidden" : "Visible"}</TableCell>
                  <TableCell>{node.isActive ? "Active" : "Inactive"}</TableCell>
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
