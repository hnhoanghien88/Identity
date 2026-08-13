import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { expect, it, vi } from "vitest";
import { ActionFormDialog } from "../../src/features/actions/components/ActionFormDialog";
import { ActionsTable } from "../../src/features/actions/components/ActionsTable";
import { DeleteActionDialog } from "../../src/features/actions/components/DeleteActionDialog";

const action = {
  id: 1,
  code: "READ",
  name: "Read",
  createdDate: "2026-08-12T00:00:00Z",
  version: 2,
};

it("renders labelled keyboard-focusable Action controls", async () => {
  const user = userEvent.setup();
  const onEdit = vi.fn();
  render(
    <ActionsTable
      items={[action]}
      totalCount={1}
      page={1}
      pageSize={20}
      sort={{ column: 3, direction: 1 }}
      onPage={() => {}}
      onPageSize={() => {}}
      onSort={() => {}}
      onEdit={onEdit}
      onDelete={() => {}}
    />,
  );
  expect(screen.getByRole("table", { name: "Actions" })).toBeInTheDocument();
  await user.click(screen.getByRole("button", { name: "Edit READ" }));
  expect(onEdit).toHaveBeenCalledWith(action);
});

it("validates create form and submits trimmed values", async () => {
  const user = userEvent.setup();
  const onSubmit = vi.fn();
  render(
    <ActionFormDialog
      open
      pending={false}
      onClose={() => {}}
      onSubmit={onSubmit}
    />,
  );
  await user.click(screen.getByRole("button", { name: "Save" }));
  expect(screen.getByText("Code is required.")).toBeInTheDocument();
  await user.type(screen.getByLabelText(/Code/), " READ ");
  await user.type(screen.getByLabelText(/Name/), " Read ");
  await user.click(screen.getByRole("button", { name: "Save" }));
  expect(onSubmit).toHaveBeenCalledWith({ code: "READ", name: "Read" });
});

it("identifies the delete target and allows cancellation", async () => {
  const user = userEvent.setup();
  const onClose = vi.fn();
  render(
    <DeleteActionDialog
      action={action}
      pending={false}
      onClose={onClose}
      onConfirm={() => {}}
    />,
  );
  expect(screen.getByText(/READ/)).toBeInTheDocument();
  await user.click(screen.getByRole("button", { name: "Cancel" }));
  expect(onClose).toHaveBeenCalled();
});
