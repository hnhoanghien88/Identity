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
      menus={[menu]}
      resources={[]}
      pending={false}
      onClose={vi.fn()}
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
