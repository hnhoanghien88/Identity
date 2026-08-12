# Data Model: Quản lý Resources

## Resource

Existing aggregate stored in `resources`; this feature adds `Version` and makes Code collation semantics explicit.

| Field | Type | Required | Rules / meaning |
|---|---|---:|---|
| Id | unsigned 64-bit integer | yes | Database-generated primary key; immutable. |
| ApplicationId | unsigned 64-bit integer | yes | References an active, non-deleted Application on create/move. |
| Code | string(120) | yes | Trimmed; unique case-insensitively with ApplicationId, including deleted rows. |
| Name | string(150) | yes | Trimmed; non-empty. |
| ResourceType | string(30) | yes | Trimmed free-form classification; non-empty. |
| Description | string(500), nullable | no | Trimmed; empty becomes null. |
| IsActive | boolean | yes | Defaults true; false excludes use in new business activity. |
| IsDeleted | boolean | yes | Defaults false; true excludes normal reads/use. |
| Version | unsigned 64-bit integer | yes | Defaults 1; concurrency token incremented once per update/delete. |
| CreatedBy | string, nullable | no | Authenticated actor according to existing audit convention. |
| CreatedDate | UTC datetime | yes | Server-stamped on create. |
| UpdatedBy | string, nullable | no | Server-stamped on update/delete. |
| UpdatedDate | UTC datetime, nullable | no | Server-stamped on update/delete. |

### Validation invariants

- ApplicationId is positive and references an active, non-deleted Application at mutation time.
- Code, Name and ResourceType cannot be null, empty or whitespace after trimming.
- Code ≤ 120, Name ≤ 150, ResourceType ≤ 30 and Description ≤ 500 characters.
- `(ApplicationId, Code)` is case-insensitively unique across active and deleted rows.
- Client cannot supply Id, audit fields, IsDeleted or server-generated Version.
- Update/delete require Version equal to stored Version.
- Deleted Resources never appear in normal search/detail and cannot be changed again.

### State transitions

```text
Create -> Active
  IsActive=true, IsDeleted=false, Version=1

Active <-> Inactive
  Update with matching Version
  Version += 1; Updated* stamped

Owned by Application A -> owned by Application B
  Application B active/non-deleted; (B, Code) unique
  same Resource Id and dependencies; Version += 1

Active/Inactive -> Deleted
  matching Version and no Permission/Menu reference
  IsDeleted=true, IsActive=false, Version += 1

Deleted -> any other state
  prohibited
```

## Application

Existing parent entity. Resource mutations validate `Id`, `IsActive=true` and `IsDeleted=false`. Search DTOs project Application Code/Name for display. This feature does not modify Application.

## Resource dependencies

| Dependent | Relationship | Delete behavior |
|---|---|---|
| Permissions | required ResourceId | Blocks soft-delete. |
| Menus | optional ResourceId | Blocks soft-delete when populated. |

Changing ApplicationId preserves Resource Id and dependency rows. Command logic must keep the mutation and dependency/application checks transactionally consistent.

## Read models

### ResourceDto

`Id`, `ApplicationId`, `ApplicationCode`, `ApplicationName`, `Code`, `Name`, `ResourceType`, nullable `Description`, `CreatedDate`, `IsActive`, `Version`.

### PagedResourcesDto

| Field | Type | Meaning |
|---|---|---|
| Items | ResourceDto[] | Current validated page. |
| TotalCount | integer | Matching non-deleted rows before paging. |
| Page | integer | 1-based page. |
| PageSize | integer | Validated size, 1–100. |

### Filter and sort

- Optional ApplicationId exact filter.
- Code, Name, ResourceType and ApplicationCode/ApplicationName support case-insensitive contains.
- Optional IsActive exact filter.
- Allow-listed sorts: Id, Application, Code, Name, ResourceType, CreatedDate, IsActive.
- Default: CreatedDate descending, then Id descending.
- Page defaults 1; PageSize defaults 20 and maximum 100.

## Migration and recovery

- Add non-null `Version` with default 1 and configure as concurrency token.
- Make Code collation explicitly case-insensitive while retaining unique index on `(ApplicationId, Code)`.
- Before collation/index change, detect case-only duplicate Codes per Application and abort with an operator report.
- Validate Permission/Menu foreign keys and existing rows before and after migration.
- Rollback removes Version and restores prior Code metadata only after confirming deployed clients no longer require Version. Back up data and coordinate application rollback because concurrency history is lost.
