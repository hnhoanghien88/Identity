import { actionsFetch } from "../../actions/api/actionsClient";

export const getRateLimitPolicies = (signal) =>
  actionsFetch("/api/rate-limiting", { signal }).then(
    (response) => response.data,
  );
export const getRateLimitEndpoints = (signal) =>
  actionsFetch("/api/rate-limiting/endpoints", { signal }).then(
    (response) => response.data,
  );
export const createRateLimitPolicy = (value) =>
  actionsFetch("/api/rate-limiting", {
    method: "POST",
    body: JSON.stringify(value),
  }).then((response) => response.data);
export const updateRateLimitPolicy = (id, value) =>
  actionsFetch(`/api/rate-limiting/${id}`, {
    method: "PUT",
    body: JSON.stringify(value),
  }).then((response) => response.data);
export const deleteRateLimitPolicy = (id, version) =>
  actionsFetch(`/api/rate-limiting/${id}?version=${version}`, {
    method: "DELETE",
  });