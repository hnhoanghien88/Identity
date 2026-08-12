import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { RolePermissionsPage } from "../../src/features/rolePermissions";
import { searchRoles } from "../../src/features/roles/api/rolesApi";
import { searchResources } from "../../src/features/resources/api/resourcesApi";
import {
  getRolePermissions,
  grantRolePermission,
  revokeRolePermission,
} from "../../src/features/rolePermissions/api/rolePermissionsApi";

vi.mock("../../src/features/roles/api/rolesApi", () => ({
  searchRoles: vi.fn(),
}));
vi.mock("../../src/features/resources/api/resourcesApi", () => ({
  searchResources: vi.fn(),
}));
vi.mock("../../src/features/rolePermissions/api/rolePermissionsApi", () => ({
  getRolePermissions: vi.fn(),
  grantRolePermission: vi.fn(),
  revokeRolePermission: vi.fn(),
}));

const roles = [
  { id: 1, code: "ADMIN" },
  { id: 2, code: "AUDITOR" },
];
const resources = [
  { id: 10, applicationCode: "PORTAL", code: "USERS" },
  { id: 11, applicationCode: "PORTAL", code: "REPORTS" },
];

describe("RolePermissionsPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    searchRoles.mockResolvedValue({ items: roles, totalCount: 2 });
    searchResources.mockResolvedValue({ items: resources, totalCount: 2 });
    getRolePermissions.mockResolvedValue({
      roleId: 1,
      resourceId: 10,
      actions: [
        { actionId: 100, code: "VIEW", name: "View", isGranted: false },
        { actionId: 101, code: "EDIT", name: "Edit", isGranted: true },
      ],
    });
    grantRolePermission.mockResolvedValue({});
    revokeRolePermission.mockResolvedValue({});
  });

  it("selects first Role and Resource and renders accessible Actions", async () => {
    render(<RolePermissionsPage />);

    expect(
      await screen.findByRole("checkbox", { name: /VIEW/ }),
    ).not.toBeChecked();
    expect(screen.getByRole("checkbox", { name: /EDIT/ })).toBeChecked();
    expect(screen.getByRole("list", { name: "Roles" })).toBeInTheDocument();
    expect(screen.getByRole("list", { name: "Resources" })).toBeInTheDocument();
    expect(getRolePermissions).toHaveBeenCalledWith(
      1,
      10,
      expect.any(AbortSignal),
    );
  });

  it("reloads Actions when the active selection changes", async () => {
    const user = userEvent.setup();
    render(<RolePermissionsPage />);
    await screen.findByRole("checkbox", { name: /VIEW/ });

    await user.click(screen.getByText("AUDITOR"));

    await waitFor(() =>
      expect(getRolePermissions).toHaveBeenCalledWith(
        2,
        10,
        expect.any(AbortSignal),
      ),
    );
    expect(screen.getByText("AUDITOR").closest(".Mui-selected")).not.toBeNull();
  });

  it("grants and revokes a Permission", async () => {
    const user = userEvent.setup();
    render(<RolePermissionsPage />);
    const view = await screen.findByRole("checkbox", { name: /VIEW/ });
    const edit = screen.getByRole("checkbox", { name: /EDIT/ });

    await user.click(view);
    await waitFor(() =>
      expect(grantRolePermission).toHaveBeenCalledWith(1, 10, 100),
    );
    await user.click(edit);
    await waitFor(() =>
      expect(revokeRolePermission).toHaveBeenCalledWith(1, 10, 101),
    );
  });

  it("rolls back an optimistic grant when saving fails", async () => {
    grantRolePermission.mockRejectedValueOnce(new Error("Save failed"));
    const user = userEvent.setup();
    render(<RolePermissionsPage />);
    const view = await screen.findByRole("checkbox", { name: /VIEW/ });

    await user.click(view);

    expect(await screen.findByText("Save failed")).toBeInTheDocument();
    expect(view).not.toBeChecked();
    expect(screen.getByRole("button", { name: "Retry" })).toBeInTheDocument();
  });
});
