# Research: Quản lý Applications

## Decision 1: CQRS follows the existing Users vertical-slice pattern

**Decision**: Use MediatR commands/queries; EF Core repository for writes and Dapper repository for read projections.

**Rationale**: This is the established repository architecture, preserves the required layer direction and makes the requested CQRS split explicit without adding dependencies.

**Alternatives considered**:

- EF Core for both reads and writes: simpler locally, but does not follow the requested or existing read/write separation.
- Direct persistence in controllers: rejected because it violates Application/Infrastructure boundaries and duplicates business rules.

## Decision 2: Add optimistic concurrency Version

**Decision**: Add a required unsigned `Version` column defaulting to 1, configure it as an EF concurrency token, expose it in DTOs, require it for update/delete and increment it for every mutation.

**Rationale**: Applications currently has no concurrency token, while the spec forbids silent overwrites. Comparing only in a handler is vulnerable to a race between read and save; EF concurrency enforcement closes that window.

**Alternatives considered**:

- Last-write-wins: violates FR-019.
- Request-only version comparison: insufficient race protection.
- Timestamp as token: less explicit than the repository's existing Users Version pattern.

## Decision 3: Soft-delete with explicit dependency protection

**Decision**: Delete sets `IsDeleted=true`, `IsActive=false`, audit fields and Version only after checking all Application dependencies. A dependency returns 409 with no mutation.

**Rationale**: Foreign-key Restrict does not run during an update-based soft-delete. Explicit checks are required to satisfy integrity behavior and provide a safe user-facing result.

**Alternatives considered**:

- Physical delete: risks destructive history loss and conflicts with ActiveEntity.
- Soft-delete without dependency checks: leaves active dependent configuration pointing at an unusable Application.
- Cascading soft-delete: broadens scope into related entities.

## Decision 4: Code uniqueness is case-insensitive and includes deleted rows

**Decision**: Trim Code and enforce explicit case-insensitive collation on the unique Code index; uniqueness queries include soft-deleted rows; map database unique violations to the `code` field.

**Rationale**: The spec treats case variants as duplicates. Keeping deleted Codes reserved matches the current unfiltered unique index and avoids precheck/database disagreement.

**Alternatives considered**:

- Rely on environment default collation: behavior could differ by deployment.
- Reuse Code after soft-delete: needs a different indexing design and creates identity ambiguity.
- Normalize into an extra column: unnecessary with an explicit supported collation.

## Decision 5: Use independent permission policies

**Decision**: Add policies for `Applications.View`, `.Create`, `.Update`, and `.Delete`, backed by existing JWT permission claims and granted through existing role-permission data.

**Rationale**: The spec explicitly requires independent rights. Existing Admin/Manager role groups cannot express view-only or create-without-delete access.

**Alternatives considered**:

- Copy Users role authorization: simpler but violates FR-003.
- Client-only capability checks: insecure because direct API calls bypass the UI.

## Decision 6: Mirror existing HTTP conventions

**Decision**: Use `/api/applications`, `/search`, `/{id}`, PUT and DELETE with `version`; wrap success in `ApiResponse<T>` and use Problem Details for failures.

**Rationale**: Consistency reduces client plumbing and review risk. POST search supports the established structured filter/sort object.

**Alternatives considered**:

- GET query-string search: awkward for structured filters and inconsistent with Users.
- Separate activation PATCH: unnecessary because IsActive is part of edit; any future endpoint must still carry Version and audit actor.
- 204 delete: valid, but 200 wrapper matches the existing convention.

## Decision 7: Preserve stale data on query failure

**Decision**: After a successful load, a failed reload displays an actionable error while retaining prior rows; first-load failure displays error and Retry.

**Rationale**: This directly satisfies FR-007 without pretending stale data is fresh.

**Alternatives considered**:

- Clear the table on every request: loses useful context.
- Infinite retry: hides failures and can overload an unavailable service.

## Decision 8: No new libraries

**Decision**: Use current .NET, Dapper/EF/MediatR/FluentValidation and React/MUI/Vitest tooling.

**Rationale**: Existing capabilities cover the feature, minimizing license, vulnerability and maintenance work.

**Alternatives considered**: A data-grid or server-state library is unnecessary for one screen already patterned by Users.
