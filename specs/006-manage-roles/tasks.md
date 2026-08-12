# Tasks: Quáº£n lÃ½ Roles

## Phase 1: Setup

- [X] T001 Add Roles feature/test directories under Identity-api/src, Identity-api/tests, Identity-client/src/features/roles and Identity-client/tests/roles

## Phase 2: Foundational

- [X] T002 Add Role Version concurrency field/configuration and migration in Identity-api/src/Identity.Domain/Entities/AuthorizationEntities.cs and Identity-api/src/Identity.Infrastructure
- [X] T003 [P] Define IRolesReadRepository and IRolesRepository in Identity-api/src/Identity.Application/Abstractions/Persistence
- [X] T004 [P] Define Role DTOs, filters, sorts and validation rules in Identity-api/src/Identity.Application/Roles
- [X] T005 Register Roles repositories and validators in Identity-api/src/Identity.Infrastructure/DependencyInjection.cs and Identity-api/src/Identity.Api/Program.cs

## Phase 3: User Story 1 - View and search Roles (P1)

**Goal**: Authenticated users can navigate, filter, sort and page Roles without Roles claims.

**Independent Test**: Login without Roles claims, open `/roles`, filter Application/Code/Name, sort/page; anonymous API receives 401.

- [X] T006 [P] [US1] Add Roles query and authenticated API contract tests under Identity-api/tests/*/Roles
- [X] T007 [P] [US1] Add Roles list/navigation frontend tests under Identity-client/tests/roles
- [X] T008 [US1] Implement GetRoles/GetRoleById and DapperRolesReadRepository under Identity-api/src
- [X] T009 [US1] Implement authenticated Roles search/detail endpoints in Identity-api/src/Identity.Api/Controllers/RolesController.cs
- [X] T010 [US1] Implement Roles page, filters, table, API client and `/roles` navigation under Identity-client/src

## Phase 4: User Story 2 - Create Role (P1)

**Goal**: Create a validated Role scoped to an existing Application.

**Independent Test**: Valid create persists; invalid Application/fields and duplicate Code in the same Application fail atomically.

- [X] T011 [P] [US2] Add create handler, API and Role form tests under Identity-api/tests/*/Roles and Identity-client/tests/roles
- [X] T012 [US2] Implement CreateRole command, validator and repository write under Identity-api/src
- [X] T013 [US2] Integrate Application selector and create workflow under Identity-client/src/features/roles

## Phase 5: User Story 3 - Update Role (P2)

**Goal**: Update Role fields and flags without silent concurrent overwrite.

**Independent Test**: Valid update increments Version; invalid/duplicate/stale requests leave data unchanged.

- [X] T014 [P] [US3] Add update validation/concurrency and UI tests under Identity-api/tests/*/Roles and Identity-client/tests/roles
- [X] T015 [US3] Implement UpdateRole command, validator and optimistic persistence under Identity-api/src
- [X] T016 [US3] Integrate edit workflow and stale conflict handling under Identity-client/src/features/roles

## Phase 6: User Story 4 - Delete Role (P3)

**Goal**: Confirm and physically delete only normal, unlinked Roles.

**Independent Test**: Cancel unchanged; eligible Role deletes; system, linked or stale Role returns conflict unchanged.

- [X] T017 [P] [US4] Add system/dependency/concurrency delete and dialog tests under Identity-api/tests/*/Roles and Identity-client/tests/roles
- [X] T018 [US4] Implement DeleteRole and dependency-safe physical delete under Identity-api/src
- [X] T019 [US4] Implement delete confirmation and conflict UX under Identity-client/src/features/roles

## Phase 7: Polish & Cross-Cutting Concerns

- [X] T020 [P] Add Roles accessibility and performance/authentication coverage under Identity-client/tests/roles and Identity-api/tests
- [X] T021 Validate migration/recovery and update specs/006-manage-roles/quickstart.md with results
- [X] T022 Run backend build/tests and frontend formatter/lint/tests/build, then review multi-line formatting across changed files

## Dependencies

- T001 â†’ T002-T005 â†’ US1 â†’ US2 â†’ US3 â†’ US4 â†’ polish.
- US1 supplies shared read/API/UI infrastructure. US2, US3 and US4 then deliver independently testable write increments.
- `[P]` tasks touch separate test/contracts or interfaces and may run concurrently once prior phases complete.

## Parallel Execution Examples

- Foundation: T003 and T004 in parallel after T002.
- US1: T006 and T007 before T008-T010.
- Each write story: its backend and frontend test surfaces may be prepared together before implementation.
- Polish: accessibility/client checks and backend auth/performance coverage can run independently.

## Implementation Strategy

MVP is US1 (authenticated list/search/navigation). Add create, update and protected delete in priority order. Each story must pass its independent test before the next phase. All 22 tasks use the required checkbox/ID/story/path format.
