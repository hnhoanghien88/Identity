# Implementation Plan: User Management

**Branch**: `001-manage-users` | **Date**: 2026-08-10 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-manage-users/spec.md`

## Summary

Keep Code as the independent ASCII login identifier and Email as required profile data, but remove the stored NormalizedCode and NormalizedEmail fields. Enforce case-insensitive lookup and uniqueness directly on Code and Email, migrate existing indexes/columns safely, and retain the established CQRS, MUI, authorization, pagination, soft-delete, and concurrency design.

## Technical Context

**Language/Version**: C# / .NET 10.0; JavaScript ES modules / React 19.2

**Primary Dependencies**: ASP.NET Core 10, MediatR 14.2, FluentValidation 12.1, EF Core 10 + MySQL, Dapper 2.1, MySqlConnector 2.4; React 19, Vite 8, Material UI 9

**Storage**: MySQL Identity_db; users retains case-insensitive utf8mb4_unicode_ci Code/Email columns and drops NormalizedCode/NormalizedEmail

**Testing**: Extend the existing xUnit backend and Vitest/Testing Library frontend suites; retain build and lint gates

**Target Platform**: ASP.NET Core web API and evergreen desktop/mobile browsers supported by the client

**Project Type**: Web application with separate API and React client

**Performance Goals**: 95% of list/filter/sort/page interactions show results within 2 seconds for up to 10,000 users

**Constraints**: Code matches ASCII [A-Za-z0-9._-] with no whitespace; Email is required; both compare case-insensitively without stored normalized copies; login accepts Code only; fail-closed authorization; no password leakage; server pagination; accessible MUI; separate commands/queries

**Scale/Scope**: Login plus one Users route and four CRUD journeys; role assignment, password reset, bulk operations, and activation UI are out of scope

## Constitution Check

*GATE: Passed before research and re-checked after Phase 1 design.*

| Gate | Design evidence | Status |
|------|-----------------|--------|
| Security and Identity First | Login resolves Code case-insensitively and verifies password/status; Email cannot authenticate; backend authority; write-only hashed password; delete revokes access; generic auth failures. | PASS |
| Enforced Layer Boundaries | Domain state, Application CQRS/validation/contracts, Infrastructure Dapper/EF, API HTTP/auth, client API separate from views. | PASS |
| Explicit API Contracts | contracts/users.openapi.yaml defines Code-based login and separate Code/Email CRUD shapes plus 400/401/403/404/409 behavior. | PASS |
| Tests Are Release Gates | Handler, validator, repository, API integration, and frontend page/form/API tests are required by quickstart. | PASS |
| Consistent and Accessible UX | Installed MUI core, keyboard focus, labels, responsive table/dialog, and disabled/loading/error states. | PASS |
| Migration and recovery | Preflight case-insensitive duplicates, replace normalized indexes with direct Code/Email indexes, drop normalized columns, and validate forward/rollback on representative MySQL. | PASS |
| Scope discipline | Reuse CQRS and History API; no router, DataGrid, or unrelated refactor. | PASS |

### Security and compatibility decisions

- Remove anonymous access from `POST /api/users`; public registration, if later required, is a separate feature.
- Persist only Code and Email as identity/profile fields. Configure their comparison and unique indexes to be case-insensitive, then remove NormalizedCode and NormalizedEmail.
- Login accepts Code and password only. Resolve directly by Code with case-insensitive comparison and return one generic failure for unknown Code, bad password, inactive, or deleted accounts.
- Tighten search from all roles to Admin/Manager. UI gating is convenience; API authorization is authoritative.
- User responses change from Code-as-email to separate code and email fields. Update login, Users API, Dapper projections, commands, DTOs, and client atomically; version the old contract if external consumers exist.
- Delete becomes atomic soft delete, deactivation, security/permission version rotation, and refresh-token revocation.
- Existing committed development credentials are a repository risk. Validation uses local secret overrides and adds no secrets.

### Post-design re-check

All gates remain passed. The staged schema migration is required to separate the login identifier without corrupting existing Email data. No constitution exception exists, so Complexity Tracking is omitted.

## Project Structure

### Documentation (this feature)

```text
specs/001-manage-users/
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/users.openapi.yaml
`-- tasks.md                 # Generated later by $speckit-tasks
```

### Source Code (repository root)

```text
Identity-api/
|-- src/
|   |-- Identity.Domain/Entities/Users.cs
|   |-- Identity.Application/
|   |   |-- Abstractions/Persistence/
|   |   |-- Common/{Authorization,Exceptions}/
|   |   `-- Users/{AuthenticateUser,GetUsers,GetUsersById,CreateUsers,UpdateUsers,DeleteUsers,Dtos}/
|   |-- Identity.Infrastructure/
|   |   |-- Persistence/{DapperUsersReadRepository,MySqlUsersRepository}.cs
|   |   |-- Persistence/Configurations/UsersConfiguration.cs
|   |   `-- Migrations/
|   `-- Identity.Api/{Controllers,Middleware}/
`-- tests/
    |-- Identity.Application.Tests/
    |-- Identity.Infrastructure.IntegrationTests/
    `-- Identity.Api.IntegrationTests/

Identity-client/
|-- src/
|   |-- App.jsx
|   |-- App.css
|   |-- features/auth/
|   `-- features/users/
|       |-- UsersPage.jsx
|       |-- components/{UsersTable,UsersFilters,UserFormDialog,DeleteUserDialog}.jsx
|       |-- api/{usersClient,usersApi}.js
|       `-- index.js
`-- tests/users/
```

**Structure Decision**: Keep the existing two-part web application. Extend backend Users vertical slices within their current layers; add a dedicated frontend feature folder and modify only the shell for route/menu registration. Add test projects/suites per application.

## Phase 0: Research Result

Decisions, rationale, and rejected alternatives are in [research.md](./research.md). All technical unknowns are resolved.

## Phase 1: Design Result

- [data-model.md](./data-model.md): User, paged result, client state, authorization, and transitions.
- [contracts/users.openapi.yaml](./contracts/users.openapi.yaml): explicit Users HTTP contract.
- [quickstart.md](./quickstart.md): build, migration, automated tests, and CRUD smoke validation.

## Implementation Strategy

1. Preflight Code/Email data for case-insensitive duplicates and invalid values before changing indexes or dropping columns.
2. Update the User model, CQRS repositories, validators, duplicate handling, and login lookup to use Code/Email directly with case-insensitive comparison.
3. Update HTTP contracts and authentication/client forms atomically; list and filter by Code while create/edit require Email.
4. Add regression tests for ASCII Code rules, both uniqueness keys, Code-only login, generic failures, CRUD, authorization, concurrency, and deletion.
5. Run migrations, backend/frontend gates, and the revised quickstart against representative MySQL.

## Operational Risks

- Inventory old consumers of the array search response or anonymous create before release; version/transition if any external consumer exists.
- Existing Code and Email rows may collide only after case folding. Migration MUST abort before destructive changes when such duplicates exist.
- Dropping normalized columns is irreversible without reconstruction; rollback recreates/backfills them from Code/Email before restoring their old indexes.
- Database collation behavior must be verified on the deployed MySQL version so direct Code/Email indexes and queries share the same case-insensitive semantics.
- Delete and token invalidation must be consistent so a deleted account cannot regain a session through refresh.
- Rehearse migration/rollback and the count query against representative data.
- Vite currently strips `/api`, breaking Users routes. Use a development `/backend` proxy that strips only that prefix: `/backend/api/users` maps to `/api/users`, while `/backend/login` maps to `/login`.
- Smoke validation requires an existing Admin. Credentials must be provided through approved local secret/config channels.
