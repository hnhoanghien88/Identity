import { beforeEach, describe, expect, it, vi } from "vitest";

vi.mock("../../src/features/auth/session", () => ({
  getSession: () => ({ accessToken: "token" }),
  publishSession: vi.fn(),
  clearSession: vi.fn(),
}));
vi.mock("../../src/features/auth/api/refresh", () => ({
  refreshSession: vi.fn(),
}));

describe("users API transport", () => {
  beforeEach(() => {
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
});
