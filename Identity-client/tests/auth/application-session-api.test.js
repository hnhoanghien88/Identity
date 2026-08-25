import { beforeEach, describe, expect, it, vi } from "vitest";

vi.mock("../../src/features/auth/api/authorization", () => ({
  applicationCode: "Business",
  getAuthorization: vi.fn().mockResolvedValue({
    roles: [],
    permissions: [],
    menus: [],
  }),
}));

import { logout } from "../../src/features/auth/api/logout";
import { refreshSession } from "../../src/features/auth/api/refresh";

describe("application refresh session API", () => {
  beforeEach(() => {
    global.fetch = vi.fn();
  });

  it("refreshes the cookie scoped to the configured application", async () => {
    global.fetch.mockResolvedValue({
      ok: true,
      json: async () => ({
        accessToken: "token",
        accessTokenExpiresAtUtc: "2030-01-01T00:00:00Z",
      }),
    });

    await refreshSession();

    expect(global.fetch).toHaveBeenCalledWith(
      "/backend/refresh?applicationCode=Business",
      expect.objectContaining({ method: "POST", credentials: "include" }),
    );
  });

  it("logs out the cookie scoped to the configured application", async () => {
    global.fetch.mockResolvedValue({ ok: true });

    await logout();

    expect(global.fetch).toHaveBeenCalledWith(
      "/backend/logout?applicationCode=Business",
      expect.objectContaining({ method: "POST", credentials: "include" }),
    );
  });
});
