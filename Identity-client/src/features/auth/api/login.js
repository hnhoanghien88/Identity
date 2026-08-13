import { getAuthorization } from "./authorization";

const INVALID_CREDENTIALS =
  "Code ho\u1eb7c m\u1eadt kh\u1ea9u kh\u00f4ng \u0111\u00fang.";
const SERVER_ERROR =
  "M\u00e1y ch\u1ee7 \u0111ang g\u1eb7p l\u1ed7i c\u1ea5u h\u00ecnh ho\u1eb7c kh\u00f4ng k\u1ebft n\u1ed1i \u0111\u01b0\u1ee3c c\u01a1 s\u1edf d\u1eef li\u1ec7u.";
const LOGIN_ERROR = "Kh\u00f4ng th\u1ec3 \u0111\u0103ng nh\u1eadp.";

export async function login(credentials) {
  const response = await fetch("/backend/login", {
    method: "POST",
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(credentials),
  });

  if (!response.ok) {
    const problem = await response.json().catch(() => null);

    if (response.status === 401) {
      throw new Error(INVALID_CREDENTIALS);
    }

    if (response.status >= 500) {
      throw new Error(SERVER_ERROR);
    }

    throw new Error(problem?.detail || LOGIN_ERROR);
  }

  const session = await response.json();
  const authorization = await getAuthorization(session.accessToken);
  return { ...session, authorization };
}

