import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { MenusTreeTable } from "../../src/features/menus/components/MenusTreeTable";

const tree = [
  {
    id: 1,
    name: "Root",
    code: "ROOT",
    sortOrder: 0,
    isActive: true,
    isVisible: true,
    children: [
      {
        id: 2,
        name: "Child",
        code: "CHILD",
        sortOrder: 0,
        isActive: true,
        isVisible: true,
        children: [],
      },
    ],
  },
];

describe("MenusTreeTable", () => {
  it("exposes recursive rows and accessible expansion", async () => {
    const onToggle = vi.fn();
    const { rerender } = render(
      <MenusTreeTable menus={tree} expanded={new Set()} onToggle={onToggle} />,
    );
    expect(screen.queryByText("Child")).not.toBeInTheDocument();
    expect(
      screen.getByRole("columnheader", { name: "IsVisible" }),
    ).toBeInTheDocument();
    expect(
      screen.getByRole("columnheader", { name: "IsActive" }),
    ).toBeInTheDocument();
    expect(screen.getByText("Hidden")).toBeInTheDocument();
    expect(screen.getByText("Active")).toBeInTheDocument();
    await userEvent.click(screen.getByRole("button", { name: "Expand Root" }));
    expect(onToggle).toHaveBeenCalledWith(1);
    rerender(
      <MenusTreeTable
        menus={tree}
        expanded={new Set([1])}
        onToggle={onToggle}
      />,
    );
    expect(screen.getByText("Child")).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: "Collapse Root" }),
    ).toHaveAttribute("aria-expanded", "true");
  });

  it("renders NULL Resource and Route safely and keeps editing available", async () => {
    const onEdit = vi.fn();
    render(
      <MenusTreeTable
        menus={tree}
        expanded={new Set()}
        onToggle={vi.fn()}
        canEdit
        onEdit={onEdit}
      />,
    );

    expect(screen.getAllByText("—")).toHaveLength(2);
    expect(screen.queryByText("â€”")).not.toBeInTheDocument();

    await userEvent.click(screen.getByRole("button", { name: "Edit Root" }));

    expect(onEdit).toHaveBeenCalledWith(tree[0]);
  });
});
