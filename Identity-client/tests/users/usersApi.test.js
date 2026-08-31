import { beforeEach, describe, expect, it, vi } from "vitest";

const auth = vi.hoisted(() => ({
  session: { accessToken: "token" },
  refreshSessionCoordinated: vi.fn(),
}));

vi.mock("../../src/features/auth/session", () => ({
  getSession: () => auth.session,
  refreshSessionCoordinated: auth.refreshSessionCoordinated,
  clearSession: vi.fn(),
}));

describe("users API transport", () => {
  beforeEach(() => {
    auth.session = { accessToken: "token" };
    auth.refreshSessionCoordinated.mockReset();
    global.fetch = vi.fn();
  });
  it("sends bearer authentication and parses a paged result", async () => {
    global.fetch.mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => ({
        data: { items: [], totalCount: 0, page: 1, pageSize: 20 },
      }),
    });
    const { searchUsers } =
      await import("../../src/features/users/api/usersApi");
    const result = await searchUsers({ page: 1, pageSize: 20 });
    expect(result.totalCount).toBe(0);
    expect(global.fetch).toHaveBeenCalledWith(
      "/backend/api/users/search",
      expect.objectContaining({ method: "POST" }),
    );
  });

  it("coordinates a 401 refresh with the stale token before retrying", async () => {
    auth.refreshSessionCoordinated.mockImplementation(async () => {
      auth.session = { accessToken: "replacement-token" };
      return auth.session;
    });
    global.fetch
      .mockResolvedValueOnce({ ok: false, status: 401 })
      .mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => ({
          data: { items: [], totalCount: 0, page: 1, pageSize: 20 },
        }),
      });

    const { searchUsers } =
      await import("../../src/features/users/api/usersApi");
    await searchUsers({ page: 1, pageSize: 20 });

    expect(auth.refreshSessionCoordinated).toHaveBeenCalledWith("token");
    expect(global.fetch).toHaveBeenNthCalledWith(
      2,
      "/backend/api/users/search",
      expect.objectContaining({
        headers: expect.objectContaining({
          Authorization: "Bearer replacement-token",
        }),
      }),
    );
  });
});
