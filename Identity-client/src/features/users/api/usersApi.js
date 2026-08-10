import { usersFetch } from "./usersClient";

export const searchUsers = (request, signal) =>
  usersFetch("/api/users/search", {
    method: "POST",
    body: JSON.stringify(request),
    signal,
  }).then((response) => response.data);

export const getUser = (id) =>
  usersFetch(`/api/users/${id}`).then((response) => response.data);

export const createUser = (user) =>
  usersFetch("/api/users", {
    method: "POST",
    body: JSON.stringify(user),
  }).then((response) => response.data);

export const updateUser = (id, user) =>
  usersFetch(`/api/users/${id}`, {
    method: "PUT",
    body: JSON.stringify(user),
  }).then((response) => response.data);

export const deleteUser = (id, version) =>
  usersFetch(`/api/users/${id}?version=${version}`, {
    method: "DELETE",
  });
