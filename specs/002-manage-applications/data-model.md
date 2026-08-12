# Data Model: Quản lý Applications

## Application

Existing aggregate stored in `applications`; this feature adds `Version` and tightens Code collation metadata.

| Field | Type | Required | Rules / meaning |
|---|---|---:|---|
| Id | unsigned 64-bit integer | yes | Database-generated primary key; immutable. |
| Code | string(50) | yes | Trimmed; 1–50 chars; unique case-insensitively across active and soft-deleted records. |
| Name | string(150) | yes | Trimmed; 1–150 chars. |
| Audience | string(150) | yes | Trimmed; 1–150 chars; not unique. |
| Description | string(500), nullable | no | Trimmed when provided; empty becomes null; maximum 500 chars. |
| IsActive | boolean | yes | Defaults true; false prevents use for new business activity. |
| IsDeleted | boolean | yes | Defaults false; true hides the record from normal reads and prevents reuse. |
| Version | unsigned 64-bit integer | yes | Defaults 1; optimistic concurrency token; increments exactly once per update or delete. |
| CreatedBy | string, nullable | no | Authenticated actor identifier/email according to the existing audit convention. |
| CreatedDate | UTC datetime | yes | Server-stamped on create. |
| UpdatedBy | string, nullable | no | Server-stamped on every update/delete. |
| UpdatedDate | UTC datetime, nullable | no | Server-stamped on every update/delete. |

### Validation invariants

- Code, Name and Audience cannot be null, empty or whitespace after trimming.
- Code length ≤ 50; Name/Audience ≤ 150; Description ≤ 500.
- Database and application checks both enforce case-insensitive Code uniqueness; the database constraint is authoritative under races.
- Client-supplied audit fields, Id, IsDeleted and server-generated Version are not accepted.
- Update/delete require a Version equal to the current stored Version.
- Deleted records never appear in normal search/detail and cannot be updated/deleted again.

### State transitions

```text
Create
  -> Active: IsActive=true, IsDeleted=false, Version=1

Active <-> Inactive
  -> Update: IsActive changes, Version += 1, Updated* stamped

Active or Inactive -> Deleted
  -> only when Version matches and no blocking dependency
  -> IsDeleted=true, IsActive=false, Version += 1, Updated* stamped

Deleted -> any other state
  -> prohibited in this feature
```

## Read models

### ApplicationDto

`Id`, `Code`, `Name`, `Audience`, nullable `Description`, `CreatedDate`, `IsActive`, `Version`.

The list and detail use the same DTO for consistent edit/delete concurrency. Audit actor fields and `IsDeleted` remain internal.

### PagedApplicationsDto

| Field | Type | Meaning |
|---|---|---|
| Items | ApplicationDto[] | Current page after filters/sorts. |
| TotalCount | integer | Total matching non-deleted records before paging. |
| Page | integer | 1-based page number. |
| PageSize | integer | Requested validated page size. |

### ApplicationFilter and sort

- Optional Code, Name and Audience string filters support `contains`; optional `IsActive` boolean.
- Deleted rows are never selectable.
- Allow-listed sort columns: Id, Code, Name, Audience, CreatedDate, IsActive.
- Default order: CreatedDate descending, then Id descending.
- Page starts at 1; page size is 1–100, default 20.

## Relationships and delete protection

| Dependent data | Relationship | Delete behavior |
|---|---|---|
| Roles | direct `ApplicationId` | Blocks delete. |
| Resources | direct `ApplicationId` | Blocks delete. |
| Menus | direct `ApplicationId` | Blocks delete. |
| Permissions | direct `ApplicationId` and/or through Resources | Blocks delete. |
| RefreshTokens | direct `ApplicationId` | Blocks delete. |
| Future application-owned records | discovered through model review | Must be included before release or proven irrelevant. |

Dependency checks and mutation occur in one command-side transaction. A dependency conflict does not return counts, names or schema details.

## Migration and recovery

- Add non-null `Version` with default 1 to existing rows and configure it as a concurrency token.
- Make Code collation explicitly case-insensitive and retain unique index `UQApplicationsCode`.
- Before applying collation/index changes, detect case-only Code duplicates; abort with an operator report rather than choosing a winner.
- Rollback removes Version and restores prior Code metadata only after confirming no deployed client depends on Version. Back up the database and coordinate application rollback because concurrency history is lost.
