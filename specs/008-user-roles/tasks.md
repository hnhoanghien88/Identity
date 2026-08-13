# Tasks: Quáº£n lÃ½ User theo Role

**Input**: Design documents from `/specs/008-user-roles/`  
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

## Phase 1: Setup

**Purpose**: Establish feature folders and shared client entry points.

- [X] T001 Create backend UserRoles feature and test folders in Identity-api/src/Identity.Application/UserRoles and Identity-api/tests/*/UserRoles
- [X] T002 [P] Create frontend User Roles feature and test folders in Identity-client/src/features/userRoles and Identity-client/tests/userRoles

---

## Phase 2: Foundational

**Purpose**: Define shared contracts and persistence wiring required by every story.

- [X] T003 Define management DTOs, paging validation, queries and command records in Identity-api/src/Identity.Application/UserRoles/UserRoleRequests.cs
- [X] T004 Define IUserRolesRepository management interface in Identity-api/src/Identity.Application/Abstractions/Persistence/IUserRolesRepository.cs
- [X] T005 Implement shared active Role/User validation and persistence query helpers in Identity-api/src/Identity.Infrastructure/Persistence/MySqlUserRolesRepository.cs
- [X] T006 Register IUserRolesRepository in Identity-api/src/Identity.Infrastructure/DependencyInjection.cs
- [X] T007 [P] Implement authenticated HTTP client and response parsing in Identity-client/src/features/userRoles/api/userRolesClient.js and Identity-client/src/features/userRoles/api/userRolesApi.js

**Checkpoint**: Shared contracts compile and all stories can build on the repository/client boundary.

---

## Phase 3: User Story 1 - Xem Users theo Role (Priority: P1) ðŸŽ¯ MVP

**Goal**: Display two columns and load only members of the active Role.

**Independent Test**: Open `/user-roles` with known memberships, switch Roles rapidly, and verify only the active Role's members appear with correct loading/empty/error states.

- [ ] T008 [P] [US1] Add application handler tests for member paging, validation and filtering in Identity-api/tests/Identity.Application.Tests/UserRoles/UserRoleQueryHandlersTests.cs
- [ ] T009 [P] [US1] Add persistence contract tests for active Role/member filtering and stable paging in Identity-api/tests/Identity.Infrastructure.IntegrationTests/UserRoles/UserRolesPersistenceContractTests.cs
- [ ] T010 [P] [US1] Add authenticated members endpoint contract tests in Identity-api/tests/Identity.Api.IntegrationTests/UserRoles/UserRolesControllerContractTests.cs
- [X] T011 [US1] Implement member query handler in Identity-api/src/Identity.Application/UserRoles/UserRoleRequests.cs
- [X] T012 [US1] Implement paged member projection in Identity-api/src/Identity.Infrastructure/Persistence/MySqlUserRolesRepository.cs
- [X] T013 [US1] Implement GET /api/user-roles endpoint in Identity-api/src/Identity.Api/Controllers/UserRolesController.cs
- [ ] T014 [P] [US1] Add frontend tests for default Role, rapid switching and list states in Identity-client/tests/userRoles/UserRolesPage.test.jsx
- [X] T015 [US1] Implement reusable Roles selection and members column in Identity-client/src/features/userRoles/components/RoleSelectionColumn.jsx and Identity-client/src/features/userRoles/components/RoleMembersColumn.jsx
- [X] T016 [US1] Implement request cancellation, generation guard and active Role state in Identity-client/src/features/userRoles/UserRolesPage.jsx
- [X] T017 [US1] Export feature and register authenticated route/menu in Identity-client/src/features/userRoles/index.js and Identity-client/src/App.jsx
- [X] T018 [US1] Add responsive two-column User Roles styles in Identity-client/src/App.css

**Checkpoint**: User Story 1 works independently as a read-only MVP.

---

## Phase 4: User Story 2 - ThÃªm nhiá»u Users vÃ o Role (Priority: P1)

**Goal**: Search/select multiple eligible Users and persist all memberships safely in one save.

**Independent Test**: Select three candidates, save and reload; all appear once. Retry/concurrent assignment creates no duplicates; a failed save preserves dialog selection.

- [ ] T019 [P] [US2] Add application tests for candidate queries and batch validation/idempotency in Identity-api/tests/Identity.Application.Tests/UserRoles/UserRoleAssignmentHandlersTests.cs
- [ ] T020 [P] [US2] Extend persistence contract tests with candidates, transactional batch add and duplicate races in Identity-api/tests/Identity.Infrastructure.IntegrationTests/UserRoles/UserRolesPersistenceContractTests.cs
- [ ] T021 [P] [US2] Extend API contract tests with candidates and authenticated batch add outcomes in Identity-api/tests/Identity.Api.IntegrationTests/UserRoles/UserRolesControllerContractTests.cs
- [X] T022 [US2] Implement candidates and batch assignment handlers in Identity-api/src/Identity.Application/UserRoles/UserRoleRequests.cs
- [X] T023 [US2] Implement candidate paging and transactional idempotent batch assignment in Identity-api/src/Identity.Infrastructure/Persistence/MySqlUserRolesRepository.cs
- [X] T024 [US2] Implement candidates GET and assignment POST endpoints in Identity-api/src/Identity.Api/Controllers/UserRolesController.cs
- [ ] T025 [P] [US2] Add dialog tests for search, paging, multi-select, disabled/pending save and error preservation in Identity-client/tests/userRoles/AddUsersDialog.test.jsx
- [X] T026 [US2] Implement searchable paged multi-select dialog in Identity-client/src/features/userRoles/components/AddUsersDialog.jsx
- [X] T027 [US2] Integrate candidate loading, batch save, success reload and error recovery in Identity-client/src/features/userRoles/UserRolesPage.jsx

**Checkpoint**: User Story 2 adds multiple Users without duplicates or partial writes.

---

## Phase 5: User Story 3 - XÃ³a User khá»i Role (Priority: P1)

**Goal**: Confirm and remove one membership without modifying the User account.

**Independent Test**: Cancel then confirm deletion; only the confirmed membership disappears, the User account remains and becomes an add candidate; failures keep the row.

- [ ] T028 [P] [US3] Add application tests for idempotent removal and input validation in Identity-api/tests/Identity.Application.Tests/UserRoles/UserRoleRemovalHandlerTests.cs
- [ ] T029 [P] [US3] Extend persistence contract tests for scoped removal, retry and concurrency in Identity-api/tests/Identity.Infrastructure.IntegrationTests/UserRoles/UserRolesPersistenceContractTests.cs
- [ ] T030 [P] [US3] Extend API contract tests with authenticated DELETE outcomes in Identity-api/tests/Identity.Api.IntegrationTests/UserRoles/UserRolesControllerContractTests.cs
- [X] T031 [US3] Implement removal handler in Identity-api/src/Identity.Application/UserRoles/UserRoleRequests.cs
- [X] T032 [US3] Implement idempotent scoped membership removal in Identity-api/src/Identity.Infrastructure/Persistence/MySqlUserRolesRepository.cs
- [X] T033 [US3] Implement DELETE /api/user-roles/{roleId}/{userId} in Identity-api/src/Identity.Api/Controllers/UserRolesController.cs
- [ ] T034 [P] [US3] Add confirmation, cancellation and failure UI tests in Identity-client/tests/userRoles/RemoveUserDialog.test.jsx
- [X] T035 [US3] Implement accessible confirmation dialog in Identity-client/src/features/userRoles/components/RemoveUserDialog.jsx
- [X] T036 [US3] Integrate delete state, member reload and error recovery in Identity-client/src/features/userRoles/UserRolesPage.jsx

**Checkpoint**: All three stories work independently and together.

---

## Phase 6: Polish & Cross-Cutting Concerns

- [ ] T037 [P] Update API usage examples in Identity-api/src/Identity.Api/Identity.Api.http and feature validation references in specs/008-user-roles/quickstart.md
- [ ] T038 Run dotnet format/build/tests and resolve failures across Identity-api
- [X] T039 Run Prettier, oxlint, Vitest and production build and resolve failures across Identity-client
- [X] T040 Review authentication, no-policy behavior, parameterized persistence, keyboard/focus labels and multi-line JSX against .specify/memory/constitution.md

---

## Dependencies & Execution Order

- Setup (T001â€“T002) â†’ Foundational (T003â€“T007) â†’ US1 (T008â€“T018).
- US2 depends on the member/Role page shell from US1, then T019â€“T024 precede T025â€“T027 integration.
- US3 depends on the member list from US1 but is otherwise independent of US2; T028â€“T033 precede T034â€“T036 integration.
- Polish (T037â€“T040) follows all selected stories.

## Parallel Opportunities

- T002 can run alongside T001; T007 can run after client folder setup while T003â€“T006 build backend foundations.
- Within US1, T008â€“T010 and T014 are parallel test work; backend implementation T011â€“T013 can run alongside frontend component work T015.
- Within US2, T019â€“T021 and T025 are parallel before their corresponding implementations.
- Within US3, T028â€“T030 and T034 are parallel before their corresponding implementations.

## Implementation Strategy

1. Complete Setup and Foundational phases.
2. Deliver US1 as the independently testable read-only MVP.
3. Add US2 batch assignment, then US3 removal (or implement US3 in parallel after US1).
4. Run all release gates and the quickstart scenarios before completion.

## Format Validation

All 40 tasks use the required checkbox, sequential ID, optional `[P]`, required story label in story phases, and explicit file paths.
