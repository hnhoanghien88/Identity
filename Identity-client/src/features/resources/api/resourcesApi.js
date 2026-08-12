import { resourcesFetch } from "./resourcesClient";

export const searchResources = (request, signal) =>
  resourcesFetch("/api/resources/search", {
    method: "POST",
    body: JSON.stringify(request),
    signal,
  }).then((response) => response.data);

export const getResource = (id) =>
  resourcesFetch(`/api/resources/${id}`).then((response) => response.data);

export const createResource = (resource) =>
  resourcesFetch("/api/resources", {
    method: "POST",
    body: JSON.stringify(resource),
  }).then((response) => response.data);

export const updateResource = (id, resource) =>
  resourcesFetch(`/api/resources/${id}`, {
    method: "PUT",
    body: JSON.stringify(resource),
  }).then((response) => response.data);

export const deleteResource = (id, version) =>
  resourcesFetch(`/api/resources/${id}?version=${version}`, {
    method: "DELETE",
  });
