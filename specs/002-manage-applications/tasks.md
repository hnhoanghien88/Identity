# Tasks: Quáº£n lÃ½ Applications

**Input**: Design documents from `/specs/002-manage-applications/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/applications.openapi.yaml, quickstart.md

**Tests**: Required by the project constitution because this feature changes authorization, validation, persistence, API contracts and protected frontend behavior. Write story tests first and confirm they fail for the intended reason before implementation.

**Organization**: Tasks are grouped by user story so each increment is independently implementable and testable.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it changes different files and does not depend on unfinished tasks in the same phase.
- **[Story]**: Maps a task to US1, US2, US3 or US4 from spec.md.
- Every task includes an exact repository-relative file path.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish the Applications feature locations and shared test organization without adding dependencies.

- [X] T001 Create Applications vertical-slice and DTO directories under `Identity-api/src/Identity.Application/Applications/` matching the structure in `specs/002-manage-applications/plan.md`
- [X] T002 [P] Create backend Applications test directories under `Identity-api/tests/Identity.Application.Tests/Applications/`, `Identity-api/tests/Identity.Infrastructure.IntegrationTests/Applications/`, and `Identity-api/tests/Identity.Api.IntegrationTests/Applications/`
- [X] T003 [P] Create frontend feature and test directories under `Identity-client/src/features/applications/` and `Identity-client/tests/applications/`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Add schema, concurrency, shared CQRS contracts and fail-closed permission infrastructure required by every story.

**âš ï¸ CRITICAL**: No user story implementation starts until this phase passes its tests.

- [X] T004 Add `Version` to Applications and its EF concurrency/default/case-insensitive Code configuration in `Identity-api/src/Identity.Domain/Entities/AuthorizationEntities.cs` and `Identity-api/src/Identity.Infrastructure/Persistence/Configurations/SchemaConfigurations.cs`
- [X] T005 Add the Applications Version/collation migration with duplicate preflight and reversible Down behavior in `Identity-api/src/Identity.Infrastructure/Migrations/`
- [X] T006 [P] Add Applications EF metadata and migration regression tests for required lengths, Code collation/unique index, Version concurrency/default, existing-row backfill and rollback safety in `Identity-api/tests/Identity.Infrastructure.IntegrationTests/Applications/ApplicationSchemaTests.cs`
- [X] T007 [P] Define `ApplicationDto` and `PagedApplicationsDto` read contracts in `Identity-api/src/Identity.Application/Applications/Dtos/ApplicationDto.cs` and `Identity-api/src/Identity.Application/Applications/Dtos/PagedApplicationsDto.cs`
- [X] T008 [P] Define query/write persistence interfaces including dependency and case-insensitive uniqueness checks in `Identity-api/src/Identity.Application/Abstractions/Persistence/IApplicationsReadRepository.cs` and `Identity-api/src/Identity.Application/Abstractions/Persistence/IApplicationsRepository.cs`
- [X] T009 [P] Define permission constants and policy names for `Applications.View`, `Applications.Create`, `Applications.Update`, and `Applications.Delete` in `Identity-api/src/Identity.Application/Common/Authorization/ApplicationPermissions.cs`
- [X] T010 Add JWT permission policy registration and fail-closed handlers in `Identity-api/src/Identity.Api/Authorization/ApplicationPermissionPolicies.cs` and wire them in `Identity-api/src/Identity.Api/Program.cs`
- [X] T011 Normalize authentication challenge and forbidden responses to safe RFC 7807 bodies in `Identity-api/src/Identity.Api/Authorization/ProblemDetailsAuthorizationResults.cs` and `Identity-api/src/Identity.Api/Program.cs`
- [X] T012 [P] Add API integration tests proving each Applications policy accepts only its permission claim and returns safe 401/403 without data leakage in `Identity-api/tests/Identity.Api.IntegrationTests/Applications/ApplicationAuthorizationTests.cs`
- [X] T013 Implement EF command repository with trimmed values, case-insensitive unique conflict mapping, optimistic concurrency mapping, audit support, soft-delete primitives and dependency checks in `Identity-api/src/Identity.Infrastructure/Persistence/MySqlApplicationsRepository.cs`
- [X] T014 Register both Applications persistence interfaces in `Identity-api/src/Identity.Infrastructure/DependencyInjection.cs`
- [X] T015 [P] Add authenticated Applications client transport with bearer/refresh/error-field mapping consistent with the existing Users client in `Identity-client/src/features/applications/api/applicationsClient.js`
- [ ] T016 [P] Add client transport tests for token attachment, refresh retry, validation Problem Details, 401/403 and 409 mapping in `Identity-client/tests/applications/applicationsClient.test.js`

**Checkpoint**: Migration, optimistic concurrency, permissions, errors, CQRS interfaces and transport foundation are ready.

---

## Phase 3: User Story 1 - Xem vÃ  tÃ¬m danh sÃ¡ch Applications (Priority: P1) ðŸŽ¯ MVP

**Goal**: Replace the Applications welcome page with an authorized, pageable/filterable/sortable list read from non-deleted Applications.

**Independent Test**: Sign in with `Applications.View`, open the existing Application menu and verify Code, Name, Audience, status and actions load from the database; filters, sorting, paging, empty state and non-destructive retry behave correctly. Direct access without View returns no data.

### Tests for User Story 1

- [X] T017 [P] [US1] Add query handler tests for default ordering, filter/sort/page validation, maximum page size and deleted-row exclusion in `Identity-api/tests/Identity.Application.Tests/Applications/GetApplicationsTests.cs`
- [ ] T018 [P] [US1] Add Dapper read integration tests for detail/search projections, parameterized Code/Name/Audience/status filters, total count, paging, sort allow-list and deleted exclusion in `Identity-api/tests/Identity.Infrastructure.IntegrationTests/Applications/ApplicationReadRepositoryTests.cs`
- [X] T019 [P] [US1] Add API contract tests for `POST /api/applications/search` and `GET /api/applications/{id}`, including response envelopes, 400/401/403/404 and View policy in `Identity-api/tests/Identity.Api.IntegrationTests/Applications/ApplicationQueryContractTests.cs`
- [X] T020 [P] [US1] Add frontend tests for menu/direct-route View gating, initial load, filters, sort, paging, empty state, StrictMode-safe cancellation and stale-row retention on retryable failure in `Identity-client/tests/applications/ApplicationList.test.jsx`

### Implementation for User Story 1

- [X] T021 [P] [US1] Implement query/filter/sort records and validation in `Identity-api/src/Identity.Application/Applications/GetApplications/GetApplicationsQuery.cs` and `Identity-api/src/Identity.Application/Applications/GetApplications/GetApplicationsQueryHandler.cs`
- [X] T022 [P] [US1] Implement non-deleted detail query in `Identity-api/src/Identity.Application/Applications/GetApplicationById/GetApplicationByIdQuery.cs` and `Identity-api/src/Identity.Application/Applications/GetApplicationById/GetApplicationByIdQueryHandler.cs`
- [X] T023 [US1] Implement parameterized Dapper detail/search/count and allow-listed sort mapping in `Identity-api/src/Identity.Infrastructure/Persistence/DapperApplicationsReadRepository.cs`
- [X] T024 [US1] Add View-protected search and detail actions with `ApiResponse<T>` envelopes in `Identity-api/src/Identity.Api/Controllers/ApplicationsController.cs`
- [X] T025 [P] [US1] Implement `searchApplications` and `getApplication` calls in `Identity-client/src/features/applications/api/applicationsApi.js`
- [X] T026 [P] [US1] Implement accessible Code/Name/Audience/status filters in `Identity-client/src/features/applications/components/ApplicationsFilters.jsx`
- [X] T027 [P] [US1] Implement responsive sortable/paged Applications table and empty/loading affordances in `Identity-client/src/features/applications/components/ApplicationsTable.jsx`
- [X] T028 [US1] Implement Applications page query state, non-destructive load errors and Retry behavior in `Identity-client/src/features/applications/ApplicationsPage.jsx`
- [X] T029 [US1] Export the feature and replace the `/applications` welcome card while preserving the existing menu route and View capability gating in `Identity-client/src/features/applications/index.js` and `Identity-client/src/App.jsx`

**Checkpoint**: US1 is independently usable as the MVP read-only Applications management screen.

---

## Phase 4: User Story 2 - Táº¡o Application (Priority: P1)

**Goal**: Allow a principal with `Applications.Create` to create a validated, audited active Application and see it in the list.

**Independent Test**: From the Applications screen, create a valid Application and reload to prove persistence, Version 1 and audit stamping; verify required/length/case-insensitive duplicate errors create no partial row and duplicate submission is blocked.

### Tests for User Story 2

- [X] T030 [P] [US2] Add validator/handler unit tests for trimming, required/length rules, nullable Description, default state, Version 1, audit and duplicate Code conflict in `Identity-api/tests/Identity.Application.Tests/Applications/CreateApplicationTests.cs`
- [ ] T031 [P] [US2] Add EF repository integration tests for atomic create, case-insensitive unique race mapping and audit persistence in `Identity-api/tests/Identity.Infrastructure.IntegrationTests/Applications/ApplicationCreatePersistenceTests.cs`
- [X] T032 [P] [US2] Add API contract tests for Create permission, 201 Location/envelope, field-keyed 400 and duplicate 409 in `Identity-api/tests/Identity.Api.IntegrationTests/Applications/ApplicationCreateContractTests.cs`
- [ ] T033 [P] [US2] Add frontend tests for create capability visibility, field validation/errors, nullable Description, pending duplicate suppression, success notice and list reload in `Identity-client/tests/applications/ApplicationCreate.test.jsx`

### Implementation for User Story 2

- [X] T034 [P] [US2] Implement create command, FluentValidation rules and handler with trim/default/audit behavior in `Identity-api/src/Identity.Application/Applications/CreateApplication/CreateApplicationCommand.cs`, `Identity-api/src/Identity.Application/Applications/CreateApplication/CreateApplicationValidator.cs`, and `Identity-api/src/Identity.Application/Applications/CreateApplication/CreateApplicationCommandHandler.cs`
- [X] T035 [US2] Add Create-policy POST action and authenticated audit actor extraction in `Identity-api/src/Identity.Api/Controllers/ApplicationsController.cs`
- [X] T036 [P] [US2] Add `createApplication` transport call in `Identity-client/src/features/applications/api/applicationsApi.js`
- [X] T037 [P] [US2] Implement accessible create/edit-capable form fields and client validation in `Identity-client/src/features/applications/components/ApplicationFormDialog.jsx`
- [X] T038 [US2] Integrate Create capability, pending/error/success states and post-create reload in `Identity-client/src/features/applications/ApplicationsPage.jsx`

**Checkpoint**: US2 can be demonstrated independently on top of the foundation; creating does not require update or delete.

---

## Phase 5: User Story 3 - Cáº­p nháº­t Application (Priority: P2)

**Goal**: Allow a principal with `Applications.Update` to edit all permitted fields, including active status, without silently overwriting a newer version.

**Independent Test**: Edit a stored Application with its current Version and verify exactly one Version increment and audit update; then save a stale copy from a second session and verify 409, unchanged current data and a reload path.

### Tests for User Story 3

- [X] T039 [P] [US3] Add validator/handler unit tests for editable fields, trim/length/duplicate rules, active-state changes, audit and stale Version conflict in `Identity-api/tests/Identity.Application.Tests/Applications/UpdateApplicationTests.cs`
- [ ] T040 [P] [US3] Add EF integration tests for concurrency-token enforcement, single Version increment, atomic failure and deleted-row rejection in `Identity-api/tests/Identity.Infrastructure.IntegrationTests/Applications/ApplicationUpdatePersistenceTests.cs`
- [X] T041 [P] [US3] Add API contract tests for Update permission, request Version, response Version, 400/404 and safe stale/duplicate 409 in `Identity-api/tests/Identity.Api.IntegrationTests/Applications/ApplicationUpdateContractTests.cs`
- [ ] T042 [P] [US3] Add frontend tests for Update capability, edit prefill, IsActive editing, Version submission, field errors, pending suppression and stale-conflict reload in `Identity-client/tests/applications/ApplicationUpdate.test.jsx`

### Implementation for User Story 3

- [X] T043 [P] [US3] Implement update command, validator and handler with optimistic concurrency, uniqueness, audit and one Version increment in `Identity-api/src/Identity.Application/Applications/UpdateApplication/UpdateApplicationCommand.cs`, `Identity-api/src/Identity.Application/Applications/UpdateApplication/UpdateApplicationValidator.cs`, and `Identity-api/src/Identity.Application/Applications/UpdateApplication/UpdateApplicationCommandHandler.cs`
- [X] T044 [US3] Add Update-policy PUT action and actor extraction in `Identity-api/src/Identity.Api/Controllers/ApplicationsController.cs`
- [X] T045 [P] [US3] Add `updateApplication` transport call including Version and IsActive in `Identity-client/src/features/applications/api/applicationsApi.js`
- [X] T046 [US3] Integrate edit selection, capability gating, Version/conflict handling and reload behavior in `Identity-client/src/features/applications/ApplicationsPage.jsx` and `Identity-client/src/features/applications/components/ApplicationFormDialog.jsx`

**Checkpoint**: US3 updates information/status safely and remains independently testable without delete.

---

## Phase 6: User Story 4 - XÃ³a Application (Priority: P3)

**Goal**: Allow a principal with `Applications.Delete` to confirm and soft-delete only an unreferenced, current-version Application.

**Independent Test**: Cancel one delete with no change, soft-delete an unreferenced Application and verify it disappears/becomes inactive, then attempt stale and dependency-blocked deletes and verify safe 409 with unchanged data.

### Tests for User Story 4

- [X] T047 [P] [US4] Add command handler tests for confirmation inputs, stale Version, soft-delete state/audit/version and safe dependency conflict in `Identity-api/tests/Identity.Application.Tests/Applications/DeleteApplicationTests.cs`
- [ ] T048 [P] [US4] Add representative MySQL integration tests for Roles/Resources/Menus/Permissions/RefreshTokens dependency checks, transactional no-change conflicts and successful soft-delete in `Identity-api/tests/Identity.Infrastructure.IntegrationTests/Applications/ApplicationDeletePersistenceTests.cs`
- [X] T049 [P] [US4] Add API contract tests for Delete permission, required query Version, 200 envelope, 404 and safe stale/dependency 409 in `Identity-api/tests/Identity.Api.IntegrationTests/Applications/ApplicationDeleteContractTests.cs`
- [ ] T050 [P] [US4] Add frontend tests for Delete capability, Code/Name confirmation, cancel, pending suppression, success reload and stale/dependency conflict messages in `Identity-client/tests/applications/ApplicationDelete.test.jsx`

### Implementation for User Story 4

- [X] T051 [P] [US4] Implement delete command and handler with current Version, explicit dependency guard, transactional soft-delete, audit and Version increment in `Identity-api/src/Identity.Application/Applications/DeleteApplication/DeleteApplicationCommand.cs` and `Identity-api/src/Identity.Application/Applications/DeleteApplication/DeleteApplicationCommandHandler.cs`
- [X] T052 [US4] Add Delete-policy DELETE action with required version and authenticated actor in `Identity-api/src/Identity.Api/Controllers/ApplicationsController.cs`
- [X] T053 [P] [US4] Add `deleteApplication` transport call with Version in `Identity-client/src/features/applications/api/applicationsApi.js`
- [X] T054 [P] [US4] Implement accessible Code/Name delete confirmation dialog with pending/error states in `Identity-client/src/features/applications/components/DeleteApplicationDialog.jsx`
- [X] T055 [US4] Integrate delete selection, capability gating, success reload and stale/dependency conflict behavior in `Identity-client/src/features/applications/ApplicationsPage.jsx`

**Checkpoint**: All four user stories are independently functional and the complete CRUD lifecycle is available.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Validate the complete feature against contracts, performance, accessibility, migration governance and repository release gates.

- [X] T056 [P] Reconcile implemented request/response/auth/error behavior with `specs/002-manage-applications/contracts/applications.openapi.yaml` and update the contract only for intentional backward-compatible corrections
- [ ] T057 [P] Add structured, non-sensitive Applications mutation/conflict logging and correlation coverage in `Identity-api/src/Identity.Api/Controllers/ApplicationsController.cs` and `Identity-api/src/Identity.Infrastructure/Persistence/MySqlApplicationsRepository.cs`
- [ ] T058 [P] Add responsive/accessibility regression coverage for keyboard focus, labels, dialog focus restore and supported viewport layouts in `Identity-client/tests/applications/ApplicationAccessibility.test.jsx`
- [ ] T059 Execute migration Up/Down/reapply and duplicate-preflight recovery validation on a disposable representative database, recording results in `specs/002-manage-applications/quickstart.md`
- [ ] T060 Execute the 10,000-record list/filter/sort/page performance scenario and record environment, dataset and p95 result in `specs/002-manage-applications/quickstart.md`
- [X] T061 Run `dotnet restore Identity-api.slnx`, `dotnet build Identity-api.slnx --no-restore`, and `dotnet test Identity-api.slnx --no-build` from `Identity-api/` and resolve all failures
- [X] T062 Run `npm ci`, `npm run lint`, `npm run test -- --run`, and `npm run build` from `Identity-client/` and resolve all failures
- [X] T063 Run Prettier on changed client files and manually review all changed C#/JS/JSX for constitution-compliant multi-line formatting using `Identity-client/src/features/applications/` and `Identity-api/src/`
- [ ] T064 Perform the complete authorization, CRUD, concurrency, dependency, network/session-expiry, keyboard and responsive smoke guide and record final evidence in `specs/002-manage-applications/quickstart.md`

---

## Dependencies & Execution Order

### Phase dependencies

- **Phase 1 Setup**: Starts immediately.
- **Phase 2 Foundational**: Depends on Setup and blocks every user story.
- **US1 (Phase 3)**: Starts after Foundational; delivers the recommended MVP.
- **US2 (Phase 4)**: Starts after Foundational; uses the shared page/table from US1 for integrated delivery, but its backend create slice is independently testable.
- **US3 (Phase 5)**: Starts after Foundational and reuses the form/list shell; does not depend on US4.
- **US4 (Phase 6)**: Starts after Foundational and reuses the list shell; does not depend on US2/US3 command handlers.
- **Polish (Phase 7)**: Starts after all stories selected for release are complete; final gates require all four.

### User story dependency graph

```text
Setup -> Foundation -> US1 (read-only MVP)
                    â”œ-> US2 (create)
                    â”œ-> US3 (update)
                    â””-> US4 (delete)

US1 + US2 + US3 + US4 -> Polish/release gates
```

For a single implementation stream, use priority order US1 â†’ US2 â†’ US3 â†’ US4. With multiple developers, backend slices for US2â€“US4 can proceed in parallel after Foundation; coordinate shared edits to `ApplicationsController.cs`, `ApplicationsPage.jsx` and `applicationsApi.js` sequentially to avoid conflicts.

### Within each user story

1. Add tests and confirm they fail for the intended missing behavior.
2. Implement Application-layer records, validation and handlers.
3. Implement persistence behavior before the endpoint consumes it.
4. Add the protected endpoint and client transport.
5. Add/integrate UI behavior.
6. Run the story's unit, integration, contract and frontend tests before its checkpoint.

## Parallel Execution Examples

### User Story 1

```text
Parallel test batch: T017, T018, T019, T020
Parallel implementation start: T021, T022, T025, T026, T027
Then: T023 -> T024; T025 + T026 + T027 -> T028 -> T029
```

### User Story 2

```text
Parallel test batch: T030, T031, T032, T033
Parallel implementation start: T034, T036, T037
Then: T034 -> T035; T036 + T037 -> T038
```

### User Story 3

```text
Parallel test batch: T039, T040, T041, T042
Parallel implementation start: T043, T045
Then: T043 -> T044; T045 -> T046
```

### User Story 4

```text
Parallel test batch: T047, T048, T049, T050
Parallel implementation start: T051, T053, T054
Then: T051 -> T052; T053 + T054 -> T055
```

## Implementation Strategy

### MVP first

1. Complete Setup and Foundation.
2. Complete US1 and its tests.
3. Stop and validate the read-only Applications screen independently.
4. Demo/deploy US1 only if a read-only release is useful and policy/migration gates pass.

### Incremental delivery

1. Foundation â†’ schema, policy and CQRS contracts ready.
2. US1 â†’ searchable read-only MVP.
3. US2 â†’ registration/create capability.
4. US3 â†’ safe editing and activation state.
5. US4 â†’ protected lifecycle completion through soft-delete.
6. Polish â†’ migration recovery, performance, accessibility and all release gates.

## Notes

- `[P]` means safe file-level parallelism only; tasks sharing `ApplicationsController.cs`, `ApplicationsPage.jsx`, `applicationsApi.js`, `Program.cs` or DI must be coordinated.
- All persistence and API contract changes require automated unit plus integration coverage under the constitution.
- Keep Dapper SQL parameterized and sort columns allow-listed; never log tokens, claims payloads or database exception details.
- Do not physically delete Applications or cascade changes into related entities.
- Commit after each task or coherent task group and validate at every story checkpoint.
