import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { RoleFormDialog } from "../../src/features/roles/components/RoleFormDialog";
import { RolesTable } from "../../src/features/roles/components/RolesTable";

const application = { id: 10, code: "PORTAL", name: "Portal" };

describe("Roles components", () => {
  it("renders accessible table and protects system Role deletion", () => {
    render(
      <RolesTable
        items={[
          {
            id: 1,
            applicationCode: "PORTAL",
            applicationName: "Portal",
            code: "ADMIN",
            name: "Admin",
            isSystemRole: true,
            isActive: true,
            createdDate: "2026-08-12",
            version: 1,
          },
        ]}
        totalCount={1}
        page={1}
        pageSize={20}
        sort={{ column: 6, direction: 1 }}
        onPage={vi.fn()}
        onPageSize={vi.fn()}
        onSort={vi.fn()}
        onEdit={vi.fn()}
        onDelete={vi.fn()}
      />,
    );
    expect(screen.getByRole("table", { name: "Roles" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Delete ADMIN" })).toBeDisabled();
  });

  it("submits Application and Role flags", async () => {
    const submit = vi.fn();
    render(
      <RoleFormDialog
        open
        applications={[application]}
        pending={false}
        onClose={vi.fn()}
        onSubmit={submit}
      />,
    );
    const user = userEvent.setup();
    await user.click(screen.getByRole("combobox", { name: /Application/ }));
    await user.click(screen.getByRole("option", { name: /PORTAL/ }));
    await user.type(screen.getByRole("textbox", { name: /Code/ }), " ADMIN ");
    await user.type(screen.getByRole("textbox", { name: /Name/ }), " Admin ");
    await user.click(screen.getByRole("button", { name: "Save" }));
    expect(submit).toHaveBeenCalledWith(
      expect.objectContaining({
        applicationId: 10,
        code: "ADMIN",
        isActive: true,
      }),
    );
  });
});
