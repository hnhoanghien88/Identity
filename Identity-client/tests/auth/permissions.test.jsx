import { render, screen } from "@testing-library/react";
import { expect, it } from "vitest";
import { ForbiddenPage } from "../../src/features/auth/ForbiddenPage";
import { hasPermission } from "../../src/features/auth/permissions";

it("matches exact permissions returned by login", () => {
  const session = {
    authorization: { permissions: ["Menus.Read", "Menus.Update"] },
  };

  expect(hasPermission(session, "Menus.Read")).toBe(true);
  expect(hasPermission(session, "Menus.Create")).toBe(false);
});

it("renders the 403 page for an unauthorized feature", () => {
  render(<ForbiddenPage />);

  expect(
    screen.getByRole("heading", { name: "403 — Bạn chưa có quyền" }),
  ).toBeInTheDocument();
});
