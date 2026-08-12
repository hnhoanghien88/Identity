import { actionsFetch } from "./actionsClient";

export const searchActions = (request, signal) =>
  actionsFetch("/api/actions/search", {
    method: "POST",
    body: JSON.stringify(request),
    signal,
  }).then((response) => response.data);
export const createAction = (action) =>
  actionsFetch("/api/actions", {
    method: "POST",
    body: JSON.stringify(action),
  }).then((response) => response.data);
export const updateAction = (id, action) =>
  actionsFetch(`/api/actions/${id}`, {
    method: "PUT",
    body: JSON.stringify(action),
  }).then((response) => response.data);
export const deleteAction = (id, version) =>
  actionsFetch(`/api/actions/${id}?version=${version}`, { method: "DELETE" });
