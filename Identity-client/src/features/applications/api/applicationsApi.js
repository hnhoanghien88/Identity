import { applicationsFetch } from "./applicationsClient";

export const searchApplications = (request, signal) =>
  applicationsFetch("/api/applications/search", {
    method: "POST",
    body: JSON.stringify(request),
    signal,
  }).then((response) => response.data);

export const getApplication = (id) =>
  applicationsFetch(`/api/applications/${id}`).then(
    (response) => response.data,
  );

export const createApplication = (application) =>
  applicationsFetch("/api/applications", {
    method: "POST",
    body: JSON.stringify(application),
  }).then((response) => response.data);

export const updateApplication = (id, application) =>
  applicationsFetch(`/api/applications/${id}`, {
    method: "PUT",
    body: JSON.stringify(application),
  }).then((response) => response.data);

export const deleteApplication = (id, version) =>
  applicationsFetch(`/api/applications/${id}?version=${version}`, {
    method: "DELETE",
  });
