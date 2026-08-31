import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, expect, it, vi } from "vitest";
import { MenusPage } from "../../src/features/menus/MenusPage";
import { searchResources } from "../../src/features/resources/api/resourcesApi";
import {
  getMenuApplications,
  getMenus,
  updateMenu,
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
  vi.clearAllMocks();
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

it("reloads Resources using the Application selected in the form", async () => {
  getMenuApplications.mockResolvedValue({
    items: [
      { id: 1, code: "APP1", name: "Application 1" },
      { id: 2, code: "APP2", name: "Application 2" },
    ],
  });

  render(
    <MenusPage
      session={{
        authorization: {
          permissions: ["Menus.Read", "Menus.Create", "Resources.Read"],
        },
      }}
    />,
  );

  await userEvent.click(
    await screen.findByRole("button", { name: "Create Menu" }),
  );
  const applicationSelectors = screen.getAllByRole("combobox", {
    name: "Application",
  });
  await userEvent.click(applicationSelectors.at(-1));
  await userEvent.click(
    screen.getByRole("option", { name: "APP2 — Application 2" }),
  );

  await waitFor(() =>
    expect(searchResources).toHaveBeenCalledWith(
      expect.objectContaining({
        filter: expect.objectContaining({ applicationId: 2 }),
      }),
      expect.any(AbortSignal),
    ),
  );
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

it("updates Order in place after Enter without refetching the tree", async () => {
  updateMenu.mockResolvedValue({
    id: 1,
    applicationId: 1,
    code: "ROOT",
    name: "Root",
    sortOrder: 12,
    isActive: true,
    isVisible: true,
    version: 2,
    children: [],
  });

  render(
    <MenusPage
      session={{
        authorization: {
          permissions: ["Menus.Read", "Menus.Update"],
        },
      }}
    />,
  );

  const input = await screen.findByRole("spinbutton", {
    name: "Order for Root",
  });
  await userEvent.clear(input);
  await userEvent.type(input, "12{Enter}");

  await waitFor(() => expect(input).toHaveValue(12));
  expect(updateMenu).toHaveBeenCalledTimes(1);
  expect(getMenus).toHaveBeenCalledTimes(1);
  expect(screen.getByRole("button", { name: "Collapse Root" })).toHaveAttribute(
    "aria-expanded",
    "true",
  );
});
