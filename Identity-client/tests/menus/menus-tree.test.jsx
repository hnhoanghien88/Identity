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
      <MenusTreeTable
        menus={tree}
        expanded={new Set()}
        onToggle={onToggle}
      />,
    );
    expect(screen.queryByText("Child")).not.toBeInTheDocument();
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
    expect(screen.getByRole("button", { name: "Collapse Root" })).toHaveAttribute(
      "aria-expanded",
      "true",
    );
  });
});
