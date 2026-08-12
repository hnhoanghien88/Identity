# Tasks: Quáº£n lÃ½ Actions

## Phase 1: Setup

- [X] T001 Add Actions feature directories under Identity-api/src and Identity-client/src/features/actions
- [X] T002 [P] Add Actions test directories under Identity-api/tests and Identity-client/tests/actions

## Phase 2: Foundational

- [X] T003 Add Version concurrency field/configuration/migration for permission_actions in Identity.Domain and Identity.Infrastructure
- [X] T004 [P] Define IActionsReadRepository and IActionsRepository in Identity-api/src/Identity.Application/Abstractions/Persistence
- [X] T005 [P] Define Action DTOs, filters, sorts and validation rules in Identity-api/src/Identity.Application/Actions
- [X] T006 Register Actions repositories and authenticated API surface in Identity-api/src/Identity.Infrastructure and Identity.Api

## Phase 3: User Story 1 - View and search (P1)

**Independent Test**: Authenticated user without Actions claims can open `/actions`, filter/sort/page; anonymous API gets 401.

- [X] T007 [P] [US1] Add Actions query/read persistence tests under Identity-api/tests/*/Actions
- [X] T008 [P] [US1] Add Actions list/navigation frontend tests under Identity-client/tests/actions
- [X] T009 [US1] Implement GetActions/GetActionById and DapperActionsReadRepository under Identity-api/src
- [X] T010 [US1] Implement authenticated Actions search/detail endpoints in Identity-api/src/Identity.Api/Controllers/ActionsController.cs
- [X] T011 [US1] Implement Actions page, filters, table, API client and `/actions` navigation under Identity-client/src

## Phase 4: User Story 2 - Create (P1)

**Independent Test**: Create valid Code/Name; missing, overlength and case-only duplicate fail without partial writes.

- [X] T012 [P] [US2] Add create handler/API/UI tests under Identity-api/tests/*/Actions and Identity-client/tests/actions
- [X] T013 [US2] Implement CreateAction command/validator/repository write under Identity-api/src
- [X] T014 [US2] Implement create endpoint/form integration under Identity-api/src/Identity.Api and Identity-client/src/features/actions

## Phase 5: User Story 3 - Update (P2)

**Independent Test**: Update valid Action increments Version; stale version returns conflict without overwrite.

- [X] T015 [P] [US3] Add update concurrency and UI tests under Identity-api/tests/*/Actions and Identity-client/tests/actions
- [X] T016 [US3] Implement UpdateAction command/validator/optimistic persistence under Identity-api/src
- [X] T017 [US3] Implement update endpoint and edit dialog integration under Identity-api/src/Identity.Api and Identity-client/src/features/actions

## Phase 6: User Story 4 - Delete (P3)

**Independent Test**: Cancel unchanged; unreferenced Action deletes; referenced/stale Action returns conflict unchanged.

- [X] T018 [P] [US4] Add delete dependency/concurrency and dialog tests under Identity-api/tests/*/Actions and Identity-client/tests/actions
- [X] T019 [US4] Implement DeleteAction and dependency-safe physical repository delete under Identity-api/src
- [X] T020 [US4] Implement delete endpoint/dialog integration under Identity-api/src/Identity.Api and Identity-client/src/features/actions

## Phase 7: Polish and validation

- [X] T021 [P] Add Actions accessibility and API authentication coverage under Identity-client/tests/actions and Identity-api/tests/Identity.Api.IntegrationTests/Actions
- [X] T022 Run backend build/tests and fix failures in Identity-api
- [X] T023 Run frontend format/lint/tests/build and fix failures in Identity-client
- [X] T024 Validate migration/recovery and update specs/004-manage-actions/quickstart.md
- [X] T025 Review spec/contract compliance and readable multi-line formatting across changed files

## Dependencies

- Phase 1 â†’ Phase 2 â†’ US1 â†’ US2 â†’ US3 â†’ US4 â†’ Polish.
- T004/T005 parallel after T003; test tasks can be authored before story implementation.
- MVP is US1 after foundational work; CRUD is incremental through US2â€“US4.

## Parallel examples

- US1: T007 and T008.
- US2: backend handler/API tests and frontend create tests within T012.
- US3: backend concurrency tests and frontend edit tests within T015.
- US4: backend dependency tests and frontend dialog tests within T018.


