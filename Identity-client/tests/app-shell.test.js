import { expect, it } from "vitest";
import {
  getSessionUser,
  getSessionUserName,
} from "../src/features/auth/sessionUser";

const tokenWithClaims = (claims) => {
  const payload = btoa(
    String.fromCharCode(...new TextEncoder().encode(JSON.stringify(claims))),
  )
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

it("reads the signed-in user code and display name from the access token", () => {
  expect(
    getSessionUser({
      accessToken: tokenWithClaims({
        code: "google.user@example.com",
        display_name: "Hoàng Hiển",
      }),
    }),
  ).toEqual({
    code: "google.user@example.com",
    displayName: "Hoàng Hiển",
  });
});
