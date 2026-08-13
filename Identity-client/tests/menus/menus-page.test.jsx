import { render, screen } from "@testing-library/react";
import { beforeEach, expect, it, vi } from "vitest";
import { MenusPage } from "../../src/features/menus/MenusPage";
import { searchResources } from "../../src/features/resources/api/resourcesApi";
import {
  getMenuApplications,
  getMenus,
} from "../../src/features/menus/api/menusApi";

vi.mock("../../src/features/resources/api/resourcesApi", () => ({
  searchResources: vi.fn(),
}));
vi.mock("../../src/features/menus/api/menusApi", () => ({
  createMenu: vi.fn(),
  deleteMenu: vi.fn(),
  getMenuApplications: vi.fn(),
  getMenus: vi.fn(),
  updateMenu: vi.fn(),
}));

beforeEach(() => {
  getMenuApplications.mockResolvedValue({
    items: [{ id: 1, code: "APP", name: "Application" }],
  });
  searchResources.mockResolvedValue({ items: [] });
  getMenus.mockResolvedValue([
    {
      id: 1,
      code: "ROOT",
      name: "Root",
      sortOrder: 0,
      isActive: true,
      isVisible: true,
      children: [
        {
          id: 2,
          code: "CHILD",
          name: "Child",
          sortOrder: 0,
          isActive: true,
          isVisible: true,
          children: [],
        },
      ],
    },
  ]);
});

it("uses a valid separator and expands the selected Application tree initially", async () => {
  render(
    <MenusPage session={{ authorization: { permissions: ["Menus.Read"] } }} />,
  );

  expect(await screen.findByText("APP — Application")).toBeInTheDocument();
  expect(await screen.findByText("Child")).toBeInTheDocument();
  expect(screen.getByRole("button", { name: "Collapse Root" })).toHaveAttribute(
    "aria-expanded",
    "true",
  );
});
