import { beforeEach, describe, expect, it, vi } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { LoginPage } from "../../src/features/auth/LoginPage";
import { login } from "../../src/features/auth/api/login";

vi.mock("../../src/features/auth/api/login", () => ({
  login: vi.fn(),
}));

describe("Code login", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: true,
        json: async () => ({ google: false }),
      }),
    );
  });

  it("submits Code and password without an Email field", async () => {
    const session = {
      accessToken: "token",
      authorization: {
        roles: ["Admin"],
        permissions: [],
      },
    };
    const success = vi.fn();
    login.mockResolvedValue(session);

    render(<LoginPage onLoginSuccess={success} />);

    await userEvent.type(
      screen.getByRole("textbox", { name: /^Code/ }),
      "admin",
    );
    await userEvent.type(screen.getByLabelText(/^Password/), "password1");
    await userEvent.click(screen.getByRole("button", { name: "Login" }));

    await waitFor(() =>
      expect(login).toHaveBeenCalledWith({
        code: "admin",
        password: "password1",
      }),
    );
    expect(screen.queryByLabelText("Email")).not.toBeInTheDocument();
    expect(success).toHaveBeenCalledWith(session);
  });

  it("shows one safe error returned for rejected credentials", async () => {
    login.mockRejectedValue(new Error("Code hoặc mật khẩu không đúng."));

    render(<LoginPage onLoginSuccess={vi.fn()} />);

    await userEvent.type(
      screen.getByRole("textbox", { name: /^Code/ }),
      "unknown",
    );
    await userEvent.type(screen.getByLabelText(/^Password/), "wrongpass");
    await userEvent.click(screen.getByRole("button", { name: "Login" }));

    expect(
      await screen.findByText("Code hoặc mật khẩu không đúng."),
    ).toBeVisible();
  });

  it("enables Google sign-in only when the backend provider is configured", async () => {
    fetch.mockResolvedValue({
      ok: true,
      json: async () => ({ google: true }),
    });

    render(<LoginPage onLoginSuccess={vi.fn()} />);

    expect(await screen.findByRole("button", { name: "Google" })).toBeEnabled();
    expect(fetch).toHaveBeenCalledWith("/backend/external-login/providers");
  });
});
