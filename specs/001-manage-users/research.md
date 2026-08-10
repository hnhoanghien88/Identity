# Research: User Management

## Store only Code and Email

**Decision**: Persist Code and Email directly and remove NormalizedCode and NormalizedEmail. Apply the broadly supported `utf8mb4_unicode_ci` case-insensitive collation to both fields and create unique indexes directly on them.

**Rationale**: This preserves the original display values, meets case-insensitive lookup/uniqueness requirements, and eliminates synchronized duplicate columns.

**Alternatives considered**: Application-only lowercase comparisons cannot prevent concurrent duplicates; functional uppercase indexes are less portable across deployed MySQL versions; `utf8mb4_0900_ai_ci` is unavailable on older compatible servers; case-sensitive keys violate the specification.

## Code normalization and validation

**Decision**: Accept only 1-50 characters matching `^[A-Za-z0-9._-]+$`. Persist the entered Code unchanged and rely on the direct Code column case-insensitive comparison/index. Reject whitespace/Unicode rather than silently stripping or transliterating it.

**Rationale**: This exactly implements an ASCII, contiguous identifier while preserving a user-recognizable display value.

**Alternatives considered**: Unicode normalization still permits Unicode; transliteration can collide or change business meaning; trimming invalid input hides data-entry errors.

## Remove normalized columns safely

**Decision**: Before changing schema, reject invalid/null Code or Email and any duplicates grouped with the target case-insensitive collation. Then create unique indexes on Code and Email, remove old normalized indexes, and drop NormalizedCode/NormalizedEmail. Rollback recreates and backfills normalized columns from the source fields.

**Rationale**: Preflight checks prevent a late unique-index failure or data loss, while rollback remains recoverable.

**Alternatives considered**: Dropping first risks losing the only usable comparison key if source data is bad; retaining unused columns contradicts the requested schema; application-only uniqueness is race-prone.

## Code-based authentication

**Decision**: Login accepts Code and password, queries the direct Code column under its case-insensitive collation, then verifies password, active/deleted state and authorization. All credential/state failures share one generic 401 response.

**Rationale**: Email must not authenticate, and a uniform failure prevents account enumeration.

**Alternatives considered**: Supporting both Code and Email creates ambiguous identifiers and violates the acceptance criteria; detailed failures leak account state.

## Existing CQRS split

**Decision**: Keep MediatR queries backed by `IUsersReadRepository`/Dapper and commands backed by `IUsersRepository`/EF Core.

**Rationale**: The repository already follows this split; it meets the CQRS request and constitution layer boundaries with minimal change.

**Alternatives considered**: Direct controller-to-EF CRUD breaks boundaries; a second CQRS library adds no required capability.

## Paged search result

**Decision**: Return `{ items, totalCount, page, pageSize }` and execute a parameterized count query with identical filters.

**Rationale**: MUI server pagination cannot derive accurate pages from the current array, particularly at 10,000 users.

**Alternatives considered**: A `hasNext` guess cannot show a correct count; loading all users harms scale and privacy.

## Authorization mapping

**Decision**: Admin and Manager may list, read, create, and update; only Admin may delete. Employees cannot access Users management. Enforce this at the API and mirror it in UI affordances.

**Rationale**: Existing roles/JWT claims support this mapping and it closes the current anonymous-create and all-role-search gaps.

**Alternatives considered**: Fine-grained permission policies were deferred because stable permission codes/policy wiring are not yet established; they can later replace role mapping without changing journeys.

## Delete lifecycle

**Decision**: Delete sets `IsDeleted=true`, `IsActive=false`, advances security/permission state, updates audit/concurrency data, and revokes refresh tokens. Every read/auth/refresh path excludes deleted users.

**Rationale**: Lifecycle flags already exist; physical deletion risks restrictive foreign keys and loses identity history.

**Alternatives considered**: Physical delete creates referential/audit risk; deactivation alone leaves deleted records visible and may leave sessions usable.

## Optimistic concurrency

**Decision**: Add a numeric `Version` concurrency token. DTOs expose it; update/delete require the expected value and stale mutations return 409.

**Rationale**: It prevents silent overwrite and is unambiguous across clients.

**Alternatives considered**: `UpdatedDate` has nullability/precision issues; last-write-wins violates the spec edge case.

## Safe errors and validation

**Decision**: Return field-keyed validation problems, stable Problem Details for 401/403/404/409, generic 500 detail, and translate unique-index races to field-specific Code or Email conflicts.

**Rationale**: The client can place actionable errors without parsing strings, while internals remain hidden.

**Alternatives considered**: Single exception messages are brittle and may disclose sensitive implementation detail.

## Lightweight MUI client

**Decision**: Add `features/users`, use MUI core Table/TablePagination/Dialog, and extend the existing History API shell. Do not add React Router or MUI DataGrid.

**Rationale**: Installed dependencies cover the feature; a feature folder separates API and view state without an app-wide rewrite.

**Alternatives considered**: Router/DataGrid dependencies add disproportionate migration and licensing/dependency surface.

## Development API proxy

**Decision**: Use Vite prefix `/backend`, strip it, and call `/backend/login` plus `/backend/api/users`.

**Rationale**: Current `/api` rewriting supports root auth routes but breaks controller routes already prefixed by `/api`.

**Alternatives considered**: `/api/api/users` is confusing; changing public auth routes is unrelated and breaking; multiple proxy rules are easier to drift.

## Automated test foundation

**Decision**: Add xUnit application/infrastructure/API tests and Vitest + Testing Library frontend tests.

**Rationale**: No test projects currently exist, while security, persistence, contract, and client behavior are constitution release gates.

**Alternatives considered**: Build/lint/manual smoke alone cannot reliably regress authorization, concurrency, soft delete, or accessible dialogs.
