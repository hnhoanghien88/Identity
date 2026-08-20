import { render, screen } from "@testing-library/react";
import { beforeEach, expect, it, vi } from "vitest";
import { UserRolesPage } from "../../src/features/userRoles/UserRolesPage";
import { searchRoles } from "../../src/features/roles/api/rolesApi";
import { getRoleMembers } from "../../src/features/userRoles/api/userRolesApi";

vi.mock("../../src/features/roles/api/rolesApi", () => ({
  searchRoles: vi.fn(),
}));

vi.mock("../../src/features/userRoles/api/userRolesApi", () => ({
  assignUsersToRole: vi.fn(),
  getRoleMembers: vi.fn(),
  removeUserFromRole: vi.fn(),
}));

beforeEach(() => {
  searchRoles.mockResolvedValue({
    items: [
      {
        id: 10,
        code: "Viewer",
        name: "Read only",
        applicationCode: "PORTAL",
        applicationName: "Customer Portal",
      },
    ],
  });
  getRoleMembers.mockResolvedValue({ items: [] });
});

it("shows the Application for every Role and the active Role heading", async () => {
  render(
    <UserRolesPage
      session={{ authorization: { permissions: ["UserRoles.Read"] } }}
    />,
  );

  expect(await screen.findByText("Viewer — Read only")).toBeInTheDocument();
  expect(screen.getByText("PORTAL — Customer Portal")).toBeInTheDocument();
  expect(
    await screen.findByRole("heading", {
      name: "Users in Viewer (PORTAL — Customer Portal)",
    }),
  ).toBeInTheDocument();
});
