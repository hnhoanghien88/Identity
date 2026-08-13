# Research: Quản lý User theo Role

## Batch assignment semantics

- **Decision**: Validate Role and all requested Users first, then add all missing memberships in one transaction; existing memberships are idempotent successes and returned separately.
- **Rationale**: Prevents unexpected partial writes, survives retry/concurrent assignment, and gives the dialog a precise result without weakening the unique constraint.
- **Alternatives considered**: One request per User causes partial states and extra latency; best-effort partial insert complicates recovery.

## Read and candidate queries

- **Decision**: Expose separate paged members and candidates queries, ordered by User Code and filtered to active, non-deleted records. Search covers Code, Name, and Email.
- **Rationale**: The lists have distinct eligibility rules and paging state. Server paging avoids loading all Users.
- **Alternatives considered**: One combined snapshot is expensive; generic users search cannot reliably exclude current members.

## Membership removal

- **Decision**: Use an idempotent DELETE keyed by Role ID and User ID; absence is a successful final state.
- **Rationale**: Retries and concurrent removal remain safe, and the operation cannot be confused with account deletion.
- **Alternatives considered**: A surrogate membership ID is unnecessary; soft-deactivation conflicts with the existing unique key on re-add.

## Repository boundary

- **Decision**: Add `IUserRolesRepository` for management; retain `IUserRolesReadRepository` solely for authorization lookup.
- **Rationale**: Preserves clear responsibilities and avoids coupling token construction to administration.
- **Alternatives considered**: Expanding the authorization repository mixes unrelated consumers.

## UI concurrency

- **Decision**: Abort obsolete requests, guard responses by generation, commit member changes after server success, and preserve dialog selection on errors.
- **Rationale**: Prevents stale Role results, false success, and lost user work.
- **Alternatives considered**: Optimistic writes require complex rollback for security-sensitive relationships.

## Schema and dependencies

- **Decision**: Reuse `user_roles` and unique `(UserId, RoleId)`; add no migration or package.
- **Rationale**: Existing schema and tools cover the feature.
- **Alternatives considered**: New schema or dependencies add risk without capability.
