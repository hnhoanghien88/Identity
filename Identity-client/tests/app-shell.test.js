import { expect, it } from "vitest";
import { getSessionUserName } from "../src/features/auth/sessionUser";

const tokenWithClaims = (claims) => {
  const payload = btoa(JSON.stringify(claims))
    .replace(/\+/g, "-")
    .replace(/\//g, "_")
    .replace(/=+$/, "");
  return `header.${payload}.signature`;
};

it("shows the signed-in user code from the session or access token", () => {
  expect(getSessionUserName({ code: "admin" })).toBe("admin");
  expect(
    getSessionUserName({ accessToken: tokenWithClaims({ code: "operator" }) }),
  ).toBe("operator");
  expect(getSessionUserName({ accessToken: "invalid" })).toBe("User");
});
