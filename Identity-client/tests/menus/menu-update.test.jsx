import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { expect, it, vi } from "vitest";
import { MenuFormDialog } from "../../src/features/menus/components/MenuFormDialog";

it("submits nullable Menu fields without converting display placeholders", async () => {
  const onSubmit = vi.fn();
  const menu = {
    id: 8,
    applicationId: 1,
    parentId: null,
    resourceId: null,
    code: "menus",
    name: "Menus",
    route: null,
    icon: null,
    sortOrder: 8,
    isVisible: true,
    isActive: true,
    version: 1,
    children: [],
  };

  render(
    <MenuFormDialog
      open
      menu={menu}
      applicationId={1}
      applications={[
        { id: 1, code: "APP1", name: "Application 1" },
        { id: 2, code: "APP2", name: "Application 2" },
      ]}
      menus={[menu]}
      resources={[]}
      lookupsLoading={false}
      pending={false}
      onClose={vi.fn()}
      onApplicationChange={vi.fn()}
      onSubmit={onSubmit}
    />,
  );

  expect(screen.getByRole("textbox", { name: "Route" })).toHaveValue("");
  expect(screen.getByRole("textbox", { name: "Icon" })).toHaveValue("");

  await userEvent.click(screen.getByRole("button", { name: "Save" }));

  expect(onSubmit).toHaveBeenCalledWith(
    expect.objectContaining({
      resourceId: null,
      route: null,
      icon: null,
      version: 1,
    }),
  );
});

it("clears dependent choices and requests lookups when Application changes", async () => {
  const onApplicationChange = vi.fn();
  const onSubmit = vi.fn();
  const menu = {
    id: 8,
    applicationId: 1,
    parentId: 3,
    resourceId: 4,
    code: "menus",
    name: "Menus",
    sortOrder: 8,
    isVisible: true,
    isActive: true,
    version: 1,
    children: [],
  };

  render(
    <MenuFormDialog
      open
      menu={menu}
      applicationId={1}
      applications={[
        { id: 1, code: "APP1", name: "Application 1" },
        { id: 2, code: "APP2", name: "Application 2" },
      ]}
      menus={[{ id: 3, name: "Parent", children: [] }, menu]}
      resources={[{ id: 4, code: "R1", name: "Resource 1" }]}
      lookupsLoading={false}
      pending={false}
      onClose={vi.fn()}
      onApplicationChange={onApplicationChange}
      onSubmit={onSubmit}
    />,
  );

  await userEvent.click(screen.getByRole("combobox", { name: "Application" }));
  await userEvent.click(
    screen.getByRole("option", { name: "APP2 — Application 2" }),
  );

  expect(onApplicationChange).toHaveBeenCalledWith(2);
  await userEvent.click(screen.getByRole("button", { name: "Save" }));
  expect(onSubmit).toHaveBeenCalledWith(
    expect.objectContaining({
      applicationId: 2,
      parentId: null,
      resourceId: null,
    }),
  );
});
