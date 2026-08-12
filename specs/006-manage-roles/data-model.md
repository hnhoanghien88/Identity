# Data Model: Quản lý Roles

## Role

| Field | Type | Required | Rules / meaning |
|---|---|---:|---|
| Id | unsigned 64-bit integer | yes | Generated, immutable. |
| ApplicationId | unsigned 64-bit integer | yes | Existing Application FK. |
| Code | string(100) | yes | Trimmed; case-insensitively unique per Application. |
| Name | string(150) | yes | Trimmed; non-empty. |
| IsSystemRole | boolean | yes | Default false; true blocks delete. |
| IsActive | boolean | yes | Default true. |
| IsDeleted | boolean | yes | Remains false; managed delete is physical. |
| Version | unsigned 64-bit integer | yes | Default 1; concurrency token. |
| Created/Updated audit | audit fields | yes/partly nullable | Server-managed. |

Create starts Version 1. Update requires matching Version and increments once. Delete rejects system, stale or referenced Roles; otherwise removes physically. Application must exist; Code is 1-100 and Name 1-150 after trim.

## Dependencies and read models

`user_roles.RoleId` and `role_permissions.RoleId` block deletion; no cascade/reassignment. `RoleDto` contains Role fields plus ApplicationCode/ApplicationName and Version. `PagedRolesDto` contains Items, TotalCount, Page, PageSize.

Filters include ApplicationIds, Code, Name, IsSystemRole and IsActive. Sort allow-list is Id, Application, Code, Name, IsSystemRole, IsActive, CreatedDate; default CreatedDate DESC/Id DESC. Page defaults to 1, size 20, max 100.

## Migration and recovery

Add Version default 1/concurrency token and explicit case-insensitive Code collation. Precheck duplicate normalized Code per Application and validate FKs. Coordinate rollback; deleted rows require backup restore.
