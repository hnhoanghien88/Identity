import { render, screen } from "@testing-library/react";
import { expect, it, vi } from "vitest";
import { ResourcesTable } from "../../src/features/resources/components/ResourcesTable";

it("exposes a labelled table and labelled keyboard-focusable actions", () => {
  render(
    <ResourcesTable
      items={[
        {
          id: 1,
          applicationCode: "PORTAL",
          applicationName: "Portal",
          code: "USERS",
          name: "Users",
          resourceType: "Api",
          createdDate: "2026-08-11T00:00:00Z",
          isActive: true,
        },
      ]}
      totalCount={1}
      page={1}
      pageSize={20}
      sort={{ column: 5, direction: 1 }}
      canEdit
      canDelete
      onPage={vi.fn()}
      onPageSize={vi.fn()}
      onSort={vi.fn()}
      onEdit={vi.fn()}
      onDelete={vi.fn()}
    />,
  );

  expect(screen.getByRole("table", { name: "Resources" })).toBeInTheDocument();
  expect(screen.getByLabelText("Edit USERS")).toBeEnabled();
  expect(screen.getByLabelText("Delete USERS")).toBeEnabled();
});
