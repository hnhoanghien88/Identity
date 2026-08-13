import { StrictMode } from "react";
import { describe, expect, it, vi } from "vitest";
import { render, screen, waitFor, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { UsersFilters } from "../../src/features/users/components/UsersFilters";
import { UserFormDialog } from "../../src/features/users/components/UserFormDialog";
import { DeleteUserDialog } from "../../src/features/users/components/DeleteUserDialog";
import { UsersPage } from "../../src/features/users/UsersPage";
import { searchUsers } from "../../src/features/users/api/usersApi";

vi.mock("../../src/features/users/api/usersApi", () => ({
  createUser: vi.fn(),
  deleteUser: vi.fn(),
  searchUsers: vi.fn(),
  updateUser: vi.fn(),
}));

describe("user management components", () => {
  it("loads users once when React StrictMode replays effects", async () => {
    searchUsers.mockResolvedValueOnce({
      items: [],
      totalCount: 0,
    });

    render(
      <StrictMode>
        <UsersPage
          session={{
            accessToken: "header.eyJzdWIiOiIxIn0.signature",
            authorization: {
              roles: ["Admin"],
              permissions: ["Users.Read"],
            },
          }}
        />
      </StrictMode>,
    );

    await waitFor(() => {
      expect(searchUsers).toHaveBeenCalledOnce();
    });
  });

  it("applies accessible user filters", async () => {
    const apply = vi.fn();
    const change = vi.fn();
    render(
      <UsersFilters
        value={{ code: "", name: "", status: "" }}
        onChange={change}
        onApply={apply}
      />,
    );
    await userEvent.click(
      screen.getByRole("button", { name: /apply filters/i }),
    );
    expect(apply).toHaveBeenCalledOnce();
    expect(screen.getByLabelText("Code")).toBeVisible();
  });

  it("does not show a password field when editing", async () => {
    render(
      <UserFormDialog
        open
        user={{
          code: "user-a",
          email: "a@example.com",
          name: "A",
          version: 1,
        }}
        pending={false}
        onClose={vi.fn()}
        onSubmit={vi.fn()}
      />,
    );
    const dialog = screen.getByRole("dialog", { name: /edit user/i });
    expect(dialog).toBeVisible();
    expect(
      screen.queryByLabelText(/initial password/i),
    ).not.toBeInTheDocument();
    await waitFor(() =>
      expect(
        within(dialog).getByRole("textbox", { name: /^Code/ }),
      ).toHaveValue("user-a"),
    );
    expect(within(dialog).getByRole("textbox", { name: /^Email/ })).toHaveValue(
      "a@example.com",
    );
  });

  it("identifies the delete target and supports cancel", async () => {
    const close = vi.fn();
    render(
      <DeleteUserDialog
        user={{ code: "user-a", name: "A" }}
        pending={false}
        onClose={close}
        onConfirm={vi.fn()}
      />,
    );
    expect(screen.getByText(/user-a/)).toBeVisible();
    await userEvent.click(screen.getByRole("button", { name: /cancel/i }));
    expect(close).toHaveBeenCalledOnce();
  });
});
