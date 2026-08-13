import { resourcesFetch } from "../../resources/api/resourcesClient";

export const getMenuApplications = (signal) =>
  resourcesFetch("/api/menus/applications", { signal }).then(
    (response) => response.data,
  );

export const getMenus = (applicationId, signal) =>
  resourcesFetch(`/api/menus?applicationId=${applicationId}`, { signal }).then(
    (response) => response.data,
  );

export const createMenu = (value) =>
  resourcesFetch("/api/menus", {
    method: "POST",
    body: JSON.stringify(value),
  }).then((response) => response.data);

export const updateMenu = (id, value) =>
  resourcesFetch(`/api/menus/${id}`, {
    method: "PUT",
    body: JSON.stringify(value),
  }).then((response) => response.data);

export const deleteMenu = (id, version) =>
  resourcesFetch(`/api/menus/${id}?version=${version}`, {
    method: "DELETE",
  });
