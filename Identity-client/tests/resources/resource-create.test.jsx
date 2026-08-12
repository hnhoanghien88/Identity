import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { expect, it, vi } from "vitest";
import { ResourceFormDialog } from "../../src/features/resources/components/ResourceFormDialog";

it("validates and submits a trimmed create payload once", async () => {
  const user = userEvent.setup();
  const submit = vi.fn();
  render(
    <ResourceFormDialog
      open
      applications={[{ id: 2, code: "PORTAL", name: "Portal" }]}
      pending={false}
      onClose={vi.fn()}
      onSubmit={submit}
    />,
  );

  await user.click(screen.getByRole("combobox", { name: /Application/ }));
  await user.click(screen.getByRole("option", { name: /PORTAL/ }));
  await user.type(screen.getByRole("textbox", { name: /Code/ }), " USERS ");
  await user.type(screen.getByRole("textbox", { name: /Name/ }), " Users ");
  await user.type(
    screen.getByRole("textbox", { name: /Resource Type/ }),
    " Api ",
  );
  await user.click(screen.getByRole("button", { name: "Save" }));

  expect(submit).toHaveBeenCalledWith({
    applicationId: 2,
    code: "USERS",
    name: "Users",
    resourceType: "Api",
    description: null,
  });
});
