import { beforeEach, describe, expect, it, vi } from "vitest";

vi.mock("../../src/features/auth/api/authorization", () => ({
  applicationCode: "Identity",
  getAuthorization: vi.fn(),
}));

import { login } from "../../src/features/auth/api/login";

describe("login rate-limit response", () => {
  beforeEach(() => {
    global.fetch = vi.fn();
  });

  it("shows the wait time from Retry-After", async () => {
    global.fetch.mockResolvedValue({
      ok: false,
      status: 429,
      headers: new Headers({ "Retry-After": "42" }),
      json: async () => ({ detail: "Too many requests" }),
    });

    await expect(login({ code: "admin", password: "wrong" })).rejects.toThrow(
      "Too many login attempts. Please wait 42 seconds before trying again.",
    );
  });

  it("uses a safe fallback when Retry-After is unavailable", async () => {
    global.fetch.mockResolvedValue({
      ok: false,
      status: 429,
      headers: new Headers(),
      json: async () => null,
    });

    await expect(login({ code: "admin", password: "wrong" })).rejects.toThrow(
      "Too many login attempts. Please wait 60 seconds before trying again.",
    );
  });
});
