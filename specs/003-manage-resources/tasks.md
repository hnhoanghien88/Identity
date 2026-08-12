# Tasks: Quản lý Resources

**Input**: Design documents from `/specs/003-manage-resources/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/resources.openapi.yaml, quickstart.md

**Tests**: Included because the constitution requires automated tests for authorization, persistence and API contracts.

**Organization**: Tasks are grouped by user story so each story can be implemented and tested independently.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it changes different files and has no incomplete dependency
- **[Story]**: Maps the task to a user story from spec.md
- Every task includes an exact file or directory path

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create feature-oriented backend, client and test locations without adding dependencies.

- [X] T001 Create Resources vertical-slice directories and placeholder-free namespace structure under Identity-api/src/Identity.Application/Resources/
- [X] T002 [P] Create Resources client feature directories under Identity-client/src/features/resources/ and Identity-client/tests/resources/
- [X] T003 [P] Create Resources test directories under Identity-api/tests/Identity.Application.Tests/Resources/, Identity-api/tests/Identity.Infrastructure.IntegrationTests/Resources/, and Identity-api/tests/Identity.Api.IntegrationTests/Resources/

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish schema, contracts, permissions and shared wiring required by every story.

**⚠️ CRITICAL**: No user story implementation starts until this phase is complete.

- [X] T004 Add Resource Version concurrency token and mapping in Identity-api/src/Identity.Domain/Entities/AuthorizationEntities.cs and Identity-api/src/Identity.Infrastructure/Persistence/Configurations/SchemaConfigurations.cs
- [X] T005 Create migration with Version default, case-insensitive scoped Code uniqueness, duplicate precheck and reversible Down path in Identity-api/src/Identity.Infrastructure/Migrations/
- [X] T006 [P] Define read/write repository contracts in Identity-api/src/Identity.Application/Abstractions/Persistence/IResourcesReadRepository.cs and Identity-api/src/Identity.Application/Abstractions/Persistence/IResourcesRepository.cs
- [X] T007 [P] Define ResourceDto, PagedResourcesDto, filters, sorts and shared normalization rules in Identity-api/src/Identity.Application/Resources/Dtos/ and Identity-api/src/Identity.Application/Resources/ResourceRules.cs
- [X] T008 [P] Define Resources.View/Create/Update/Delete constants and policies in Identity-api/src/Identity.Application/Common/Authorization/ResourcePermissions.cs and Identity-api/src/Identity.Api/Program.cs
- [X] T009 Register Resources repositories and MediatR/validation wiring in Identity-api/src/Identity.Infrastructure/DependencyInjection.cs and Identity-api/src/Identity.Api/Program.cs
- [X] T010 [P] Add migration/concurrency/unique-index regression tests in Identity-api/tests/Identity.Infrastructure.IntegrationTests/Resources/ResourceMigrationTests.cs
- [X] T011 [P] Implement client capability mapping in Identity-client/src/features/resources/capabilities.js and export surface in Identity-client/src/features/resources/index.js
- [X] T012 Add protected /resources route and Resources navigation capability gate in Identity-client/src/App.jsx

**Checkpoint**: Schema, shared contracts, authorization and route foundation are ready.

---

## Phase 3: User Story 1 - Xem và tìm danh sách Resources (Priority: P1) 🎯 MVP

**Goal**: Authorized administrators can open Resources and search, filter, sort and page non-deleted records with Application context.

**Independent Test**: Open `/resources` with Resources.View, filter across Applications/Code/Name/ResourceType/status, sort/page results, and verify direct UI/API access is denied without View.

### Tests for User Story 1

> Write these tests first and confirm they fail before implementation.

- [X] T013 [P] [US1] Add query handler tests for filters, paging, sort allow-list and deleted exclusion in Identity-api/tests/Identity.Application.Tests/Resources/GetResourcesTests.cs
- [X] T014 [P] [US1] Add Dapper search/detail projection tests with Application labels in Identity-api/tests/Identity.Infrastructure.IntegrationTests/Resources/ResourceReadPersistenceTests.cs
- [X] T015 [P] [US1] Add search/detail contract plus 401/403/View-policy tests in Identity-api/tests/Identity.Api.IntegrationTests/Resources/ResourcesReadContractTests.cs
- [X] T016 [P] [US1] Add route, loading, empty, retry, stale-row retention, filtering, sorting and paging tests in Identity-client/tests/resources/resources-list.test.jsx

### Implementation for User Story 1

- [X] T017 [P] [US1] Implement GetResources and GetResourceById queries/handlers in Identity-api/src/Identity.Application/Resources/GetResources/ and Identity-api/src/Identity.Application/Resources/GetResourceById/
- [X] T018 [US1] Implement parameterized, allow-listed Resource projections in Identity-api/src/Identity.Infrastructure/Persistence/DapperResourcesReadRepository.cs
- [X] T019 [US1] Add View-protected search/detail endpoints matching OpenAPI in Identity-api/src/Identity.Api/Controllers/ResourcesController.cs
- [X] T020 [P] [US1] Implement authenticated search/detail client calls and normalized error handling in Identity-client/src/features/resources/api/resourcesClient.js and Identity-client/src/features/resources/api/resourcesApi.js
- [X] T021 [P] [US1] Implement accessible filters and responsive table in Identity-client/src/features/resources/components/ResourcesFilters.jsx and Identity-client/src/features/resources/components/ResourcesTable.jsx
- [X] T022 [US1] Implement ResourcesPage list state, stale-row retention, retry, sorting and paging in Identity-client/src/features/resources/ResourcesPage.jsx

**Checkpoint**: User Story 1 is independently usable as the read-only MVP.

---

## Phase 4: User Story 2 - Tạo Resource (Priority: P1)

**Goal**: Authorized administrators can create a validated active Resource for an active Application.

**Independent Test**: Create a valid Resource and reload it; verify same-Application duplicate/invalid Application/invalid fields fail without partial data while the same Code in another Application succeeds.

### Tests for User Story 2

- [X] T023 [P] [US2] Add create validation, trimming, audit, Application-state and scoped uniqueness tests in Identity-api/tests/Identity.Application.Tests/Resources/CreateResourceTests.cs
- [X] T024 [P] [US2] Add create transaction, Version 1, audit and unique-race persistence tests in Identity-api/tests/Identity.Infrastructure.IntegrationTests/Resources/ResourceCreatePersistenceTests.cs
- [X] T025 [P] [US2] Add POST contract, field errors and Resources.Create policy tests in Identity-api/tests/Identity.Api.IntegrationTests/Resources/ResourcesCreateContractTests.cs
- [X] T026 [P] [US2] Add Application selector, validation, duplicate-submit and retained-form tests in Identity-client/tests/resources/resource-create.test.jsx

### Implementation for User Story 2

- [X] T027 [US2] Implement CreateResource command, validator and handler with active Application validation in Identity-api/src/Identity.Application/Resources/CreateResource/
- [X] T028 [US2] Implement transactional create, audit and unique-conflict mapping in Identity-api/src/Identity.Infrastructure/Persistence/MySqlResourcesRepository.cs
- [X] T029 [US2] Add Resources.Create-protected POST endpoint and request mapping in Identity-api/src/Identity.Api/Controllers/ResourcesController.cs
- [X] T030 [US2] Implement Application selector, create dialog, API mutation and page integration in Identity-client/src/features/resources/components/ResourceFormDialog.jsx, Identity-client/src/features/resources/api/resourcesApi.js, and Identity-client/src/features/resources/ResourcesPage.jsx

**Checkpoint**: User Story 2 can create and persist Resources independently on top of the foundation.

---

## Phase 5: User Story 3 - Cập nhật Resource (Priority: P2)

**Goal**: Authorized administrators can edit all mutable fields, move a Resource to a valid Application and receive safe concurrency conflicts.

**Independent Test**: Update/move with current Version and verify one increment; then save an older Version and verify 409, no overwrite and a reload path.

### Tests for User Story 3

- [X] T031 [P] [US3] Add update validation, move, audit and stale-Version handler tests in Identity-api/tests/Identity.Application.Tests/Resources/UpdateResourceTests.cs
- [X] T032 [P] [US3] Add atomic move, scoped duplicate and optimistic-concurrency persistence tests in Identity-api/tests/Identity.Infrastructure.IntegrationTests/Resources/ResourceUpdatePersistenceTests.cs
- [X] T033 [P] [US3] Add PUT contract, 404/409 and Resources.Update policy tests in Identity-api/tests/Identity.Api.IntegrationTests/Resources/ResourcesUpdateContractTests.cs
- [X] T034 [P] [US3] Add edit prefill, Application move, field-error and stale-conflict UI tests in Identity-client/tests/resources/resource-update.test.jsx

### Implementation for User Story 3

- [X] T035 [US3] Implement UpdateResource command, validator and handler with Version and target Application checks in Identity-api/src/Identity.Application/Resources/UpdateResource/
- [X] T036 [US3] Implement atomic update/move, Version increment and concurrency mapping in Identity-api/src/Identity.Infrastructure/Persistence/MySqlResourcesRepository.cs
- [X] T037 [US3] Add Resources.Update-protected PUT endpoint in Identity-api/src/Identity.Api/Controllers/ResourcesController.cs
- [X] T038 [US3] Integrate edit mode, Version payload, stale-form retention and reload action in Identity-client/src/features/resources/components/ResourceFormDialog.jsx and Identity-client/src/features/resources/ResourcesPage.jsx

**Checkpoint**: User Stories 1–3 support read, create and concurrency-safe update independently.

---

## Phase 6: User Story 4 - Xóa Resource (Priority: P3)

**Goal**: Authorized administrators can soft-delete unreferenced Resources while Permission/Menu references and concurrent edits remain protected.

**Independent Test**: Cancel without change, soft-delete an unreferenced Resource, and verify referenced or stale Resources return 409 with unchanged data.

### Tests for User Story 4

- [X] T039 [P] [US4] Add delete state-transition, dependency and stale-Version handler tests in Identity-api/tests/Identity.Application.Tests/Resources/DeleteResourceTests.cs
- [X] T040 [P] [US4] Add transactional Permission/Menu blocking and soft-delete persistence tests in Identity-api/tests/Identity.Infrastructure.IntegrationTests/Resources/ResourceDeletePersistenceTests.cs
- [X] T041 [P] [US4] Add DELETE contract, 404/409 and Resources.Delete policy tests in Identity-api/tests/Identity.Api.IntegrationTests/Resources/ResourcesDeleteContractTests.cs
- [X] T042 [P] [US4] Add confirmation identity, cancel, pending, success and dependency/stale-conflict UI tests in Identity-client/tests/resources/resource-delete.test.jsx

### Implementation for User Story 4

- [X] T043 [US4] Implement DeleteResource command/handler and transactional dependency-aware soft-delete repository method in Identity-api/src/Identity.Application/Resources/DeleteResource/ and Identity-api/src/Identity.Infrastructure/Persistence/MySqlResourcesRepository.cs
- [X] T044 [US4] Add Resources.Delete-protected DELETE endpoint with required Version in Identity-api/src/Identity.Api/Controllers/ResourcesController.cs
- [X] T045 [US4] Implement accessible delete confirmation and page mutation handling in Identity-client/src/features/resources/components/DeleteResourceDialog.jsx and Identity-client/src/features/resources/ResourcesPage.jsx

**Checkpoint**: All four stories are independently functional and the complete CRUD lifecycle is available.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Validate performance, accessibility, migration recovery, formatting and all release gates.

- [X] T046 [P] Add 10,000-row search performance coverage and inspect query/index behavior in Identity-api/tests/Identity.Infrastructure.IntegrationTests/Resources/ResourcePerformanceTests.cs
- [X] T047 [P] Add keyboard/focus/responsive accessibility coverage in Identity-client/tests/resources/resources-accessibility.test.jsx
- [X] T048 Execute and record migration up/down, duplicate precheck and dependency recovery validation in specs/003-manage-resources/quickstart.md
- [X] T049 Run backend build/tests and client lint/Prettier/tests/build, then fix only Resources-related failures in Identity-api/ and Identity-client/
- [X] T050 Review implementation against specs/003-manage-resources/contracts/resources.openapi.yaml, specs/003-manage-resources/data-model.md, and constitution formatting/security gates

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: Starts immediately.
- **Phase 2 Foundational**: Depends on Setup and blocks all stories.
- **Phase 3 US1**: Depends on Foundational; delivers the read-only MVP.
- **Phase 4 US2**: Depends on Foundational; integrates with US1 list for normal UX.
- **Phase 5 US3**: Depends on Foundational and reuses the form/read surface; implement after US1/US2 in a single-developer flow.
- **Phase 6 US4**: Depends on Foundational and reuses the list; implement after US1 in a single-developer flow.
- **Phase 7 Polish**: Depends on all stories selected for release.

### User Story Dependencies

```text
Setup -> Foundational
                ├── US1 Read/Search (MVP)
                ├── US2 Create
                ├── US3 Update/Move
                └── US4 Delete
All selected stories -> Polish
```

- **US1**: No story dependency after Foundational.
- **US2**: Independently testable through POST and persistence; list integration is additive.
- **US3**: Independently testable through detail/PUT and persistence; shares ResourceDto/form conventions.
- **US4**: Independently testable through detail/DELETE and persistence; shares list refresh behavior.

### Within Each User Story

1. Write tests and confirm they fail for the intended missing behavior.
2. Implement Application command/query and validation.
3. Implement Infrastructure persistence.
4. Implement API endpoint and policy.
5. Implement client API/components/page integration.
6. Run story-specific tests and verify the independent checkpoint.

### Parallel Opportunities

- T002 and T003 can run in parallel after T001 is not required.
- T006, T007, T008, T010 and T011 affect separate files and can run in parallel.
- Test tasks within each story can run in parallel.
- Query/client components T017, T020 and T021 can run in parallel after foundational DTO/contracts.
- Different stories can proceed in parallel after Foundational when separate owners coordinate shared controller, repository, form and page files.

## Parallel Examples

### User Story 1

```text
T013 Application query tests
T014 Dapper integration tests
T015 API contract/authorization tests
T016 Client list tests

After tests:
T017 Query slices
T020 Client API
T021 Filters/table components
```

### User Story 2

```text
T023 Command/validation tests
T024 Persistence tests
T025 API contract tests
T026 Client create tests
```

### User Story 3

```text
T031 Command/concurrency tests
T032 Persistence move tests
T033 API contract tests
T034 Client edit tests
```

### User Story 4

```text
T039 Delete handler tests
T040 Dependency persistence tests
T041 API contract tests
T042 Client delete tests
```

## Implementation Strategy

### MVP First

1. Complete Phase 1 Setup.
2. Complete Phase 2 Foundational.
3. Complete Phase 3 User Story 1.
4. Stop and validate read/search, authorization and performance baseline independently.
5. Demo/deploy the read-only Resources screen if appropriate.

### Incremental Delivery

1. Setup + Foundational → shared platform ready.
2. US1 → searchable read-only MVP.
3. US2 → Resource registration.
4. US3 → maintenance and Application move with concurrency.
5. US4 → protected lifecycle completion.
6. Polish → performance, accessibility, migration recovery and full release gates.

### Parallel Team Strategy

After Foundational:

- Developer A: US1 read/query/list.
- Developer B: US2 create.
- Developer C: US3 update or US4 delete.
- Coordinate edits to ResourcesController.cs, MySqlResourcesRepository.cs, ResourceFormDialog.jsx and ResourcesPage.jsx before merging.

## Notes

- [P] means separate-file work with no incomplete task dependency.
- Story labels provide traceability to spec acceptance scenarios.
- Tests must fail for the intended missing behavior before implementation.
- Preserve unrelated user changes in the dirty worktree.
- Commit after each task or logical group when requested.
