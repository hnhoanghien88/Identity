import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { expect, it, vi } from "vitest";
import { ResourceFormDialog } from "../../src/features/resources/components/ResourceFormDialog";

it("prefills edit state and sends Version for concurrency", async () => {
  const user = userEvent.setup();
  const submit = vi.fn();
  render(
    <ResourceFormDialog
      open
      resource={{
        id: 1,
        applicationId: 2,
        code: "USERS",
        name: "Users",
        resourceType: "Api",
        description: "",
        isActive: true,
        version: 4,
      }}
      applications={[{ id: 2, code: "PORTAL", name: "Portal" }]}
      pending={false}
      onClose={vi.fn()}
      onSubmit={submit}
    />,
  );

  await user.clear(screen.getByRole("textbox", { name: /Name/ }));
  await user.type(screen.getByRole("textbox", { name: /Name/ }), "People");
  await user.click(screen.getByRole("button", { name: "Save" }));

  expect(submit).toHaveBeenCalledWith(
    expect.objectContaining({
      name: "People",
      version: 4,
    }),
  );
});
