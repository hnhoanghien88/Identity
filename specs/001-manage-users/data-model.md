# Data Model: User Management

## User

Existing aggregate updated so login Code and Email are independent while lifecycle concurrency remains intact.

| Field | Type | Rules / meaning |
|-------|------|-----------------|
| Id | unsigned integer | Stable server-generated identity; never editable. |
| Code | string | Required; 1-50 ASCII characters matching [A-Za-z0-9._-]; login/display value; direct case-insensitive unique comparison. |
| Email | string | Required, valid email, maximum 254 characters; direct case-insensitive unique comparison; editable but not accepted for login. |
| DisplayName (`Name` at API boundary) | string | Required, maximum 200 characters. |
| PasswordHash | protected string | Produced only by the approved hasher from an 8-128 character create password; never returned/logged. |
| IsActive | boolean | Whether authentication is allowed; new users default active. |
| IsDeleted | boolean | Lifecycle tombstone; deleted users are excluded from reads/auth. |
| SecurityStamp | GUID | Rotated for security-sensitive lifecycle changes. |
| PermissionVersion | integer | Existing authorization version; incremented on delete. |
| Version | unsigned integer | New concurrency token; starts at 1 and increments on update/delete. |
| CreatedDate / CreatedBy | audit | Set once on creation. |
| UpdatedDate / UpdatedBy | audit | Set on update/delete from authenticated actor. |

### Invariants

- Code, Email and display name are required; Code is not trimmed into validity and rejects any whitespace or Unicode.
- Direct Code and Email constraints independently enforce case-insensitive uniqueness; no normalized copies are stored.
- Code is the only login identifier; Email cannot substitute for it.
- Password is write-only and appears only in create for this feature.
- A deleted user is inactive and cannot authenticate or refresh.
- Mutation succeeds only when expected `Version` matches persisted version.
- An actor cannot delete the User matching its token subject.

### State transitions

```text
Create(valid Code + Email) -> Active { active=true, deleted=false, version=1 }
Active --Update(expected version)--> Active { Code/Email/profile updated, version+1 }
Active --Delete by another Admin(expected version)--> Deleted
Deleted { active=false, deleted=true, version+1, tokens revoked }
Stale mutation or self-delete -> no state change + 409
```

### Relationships

- User has zero or more UserRoles; soft delete preserves their history.
- User has zero or more refresh tokens; delete revokes every usable token for the target.
- Admin/Manager read-create-update; Admin alone deletes in this release.

### Migration

- Preflight all rows for required/valid Code and Email plus duplicates under the target case-insensitive comparison; abort before schema changes on failure.
- Create direct unique indexes on Code and Email using the same case-insensitive comparison used by login and repository lookups.
- Remove the old normalized indexes, then drop NormalizedCode and NormalizedEmail.
- Forward verification: Code and Email values are unchanged, direct indexes reject mixed-case duplicates, and login resolves mixed-case Code.
- Rollback recreates NormalizedCode/NormalizedEmail, backfills them deterministically from Code/Email, recreates old indexes, and removes the direct replacement indexes.

## PagedUsers

| Field | Type | Rules |
|-------|------|-------|
| Items | UserSummary[] | Current page; excludes deleted users. |
| TotalCount | integer | Same filters, before limit/offset. |
| Page | integer | 1-based, minimum 1. |
| PageSize | integer | 1 through 100. |

`UserSummary` contains `id`, `code`, `email`, `name`, `createdDate`, `isActive`, and `version`; the table renders Code instead of Email, while edit needs Email. Security internals are never returned.

## Filter and sort

- Filter by Code contains, display-name contains, and optional active status.
- Sort by id, Code, name, created date, or active status; direction asc/desc.
- Default: created date descending then id for deterministic ordering.
- Validate page/pageSize; parameterize every filter.

## Client form state

Create holds Code, Email, name, and password only in component memory. Update holds id, Code, Email, name, and expected version. Login holds Code and password only until submission/session establishment. Clear passwords after submission attempts. Field errors map to controls; 409 offers reload of current data.
