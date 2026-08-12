import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { expect, it, vi } from "vitest";
import { DeleteResourceDialog } from "../../src/features/resources/components/DeleteResourceDialog";

it("identifies Application and Resource and supports cancel/confirm", async () => {
  const user = userEvent.setup();
  const close = vi.fn();
  const confirm = vi.fn();
  render(
    <DeleteResourceDialog
      resource={{
        applicationCode: "PORTAL",
        code: "USERS",
        name: "Users",
      }}
      pending={false}
      onClose={close}
      onConfirm={confirm}
    />,
  );

  expect(screen.getByText(/PORTAL/)).toHaveTextContent("PORTAL / USERS");
  await user.click(screen.getByRole("button", { name: "Cancel" }));
  await user.click(screen.getByRole("button", { name: "Delete" }));
  expect(close).toHaveBeenCalledOnce();
  expect(confirm).toHaveBeenCalledOnce();
});
