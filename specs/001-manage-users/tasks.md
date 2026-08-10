# Tasks: User Management

**Input**: Design artifacts in `/specs/001-manage-users/`

**Tests**: Automated tests are mandatory release gates. Add or update tests before the corresponding implementation and confirm the intended failure first.

**Organization**: This is an implementation delta for removing `NormalizedCode` and `NormalizedEmail` while preserving the delivered Users CQRS feature.

## Phase 1: Setup and Migration Preflight

**Purpose**: Capture the current schema and make the destructive transition verifiable.

- [X] T001 Record applied migrations and current `users` columns/indexes using `specs/001-manage-users/quickstart.md`
- [X] T002 Add SQL that rejects null, invalid, or case-insensitively duplicated Code/Email before schema changes in `Identity-api/src/Identity.Infrastructure/Persistence/Sql/Migrations/UserCodes/002_validate_direct_identity.sql`
- [X] T003 [P] Add migration preflight and rollback verification commands to `specs/001-manage-users/quickstart.md`

---

## Phase 2: Foundational Direct Identity Schema

**Purpose**: Establish direct case-insensitive Code/Email persistence shared by all stories.

**CRITICAL**: Complete this phase before any user story.

- [X] T004 Add failing tests for direct case-insensitive uniqueness and normalized-column absence in `Identity-api/tests/Identity.Infrastructure.IntegrationTests/Users/UserPersistenceContractTests.cs`
- [X] T005 Remove `NormalizedCode` and `NormalizedEmail` from the User entity in `Identity-api/src/Identity.Domain/Entities/Users.cs`
- [X] T006 Configure required Code and Email with `utf8mb4_unicode_ci` and direct unique indexes in `Identity-api/src/Identity.Infrastructure/Persistence/Configurations/UsersConfiguration.cs`
- [X] T007 Refactor direct Code/Email existence checks and unique-key conflict mapping in `Identity-api/src/Identity.Infrastructure/Persistence/MySqlUsersRepository.cs`
- [X] T008 Create an EF migration that runs preflight, replaces normalized indexes with direct Code/Email indexes, drops both normalized columns, and backfills them on rollback in `Identity-api/src/Identity.Infrastructure/Migrations/`
- [X] T009 Update the direct Code/Email model in `Identity-api/src/Identity.Infrastructure/Migrations/IdentityDbContextModelSnapshot.cs`
- [X] T010 Update the baseline schema to omit normalized columns in `Identity-api/src/Identity.Infrastructure/Persistence/Sql/identity_db.sql`
- [X] T011 Apply the migration and record schema verification evidence in `specs/001-manage-users/quickstart.md`

**Checkpoint**: User storage contains Code and Email only, with database-enforced case-insensitive uniqueness.

---

## Phase 3: User Story 1 - Login with Code (Priority: P1) MVP

**Goal**: Authenticate by Code and password without accepting Email or using normalized identity data.

**Independent Test**: A valid active account logs in with any Code casing; Email, bad credentials, inactive users, and deleted users receive the same safe failure.

### Tests

- [X] T012 [P] [US1] Add direct and mixed-case Code authentication tests plus Email/inactive/deleted rejection in `Identity-api/tests/Identity.Application.Tests/Users/AuthenticateUserTests.cs`
- [X] T013 [P] [US1] Add Code-only form and duplicate-submit tests in `Identity-client/tests/auth/CodeLogin.test.jsx`
- [X] T014 [P] [US1] Add API tests for generic login failures and authorization payloads in `Identity-api/tests/Identity.Api.IntegrationTests/Users/UsersControllerContractTests.cs`

### Implementation

- [X] T015 [US1] Query direct Code case-insensitively and exclude inactive/deleted users in `Identity-api/src/Identity.Infrastructure/Persistence/MySqlUsersRepository.cs`
- [X] T016 [US1] Remove normalized assumptions while preserving generic failures in `Identity-api/src/Identity.Application/Users/AuthenticateUser/AuthenticateUserQueryHandler.cs`
- [X] T017 [US1] Verify the login request sends Code and never Email in `Identity-client/src/features/auth/api/login.js`
- [X] T018 [US1] Verify Code labeling, validation, and pending-state behavior in `Identity-client/src/features/auth/LoginPage.jsx`

**Checkpoint**: Code login works independently against the migrated database.

---

## Phase 4: User Story 2 - View and Search Users (Priority: P1)

**Goal**: Authorized users can open `/users` and view, filter, sort, and paginate direct Code data.

**Independent Test**: Refresh preserves `/users`; the table shows Code, and server filtering/paging returns correct totals without deleted rows.

### Tests

- [X] T019 [P] [US2] Add direct Code filter/sort/page and deleted-row tests in `Identity-api/tests/Identity.Infrastructure.IntegrationTests/Users/UserPersistenceContractTests.cs`
- [X] T020 [P] [US2] Add menu, route-refresh, Code-column, state, filter, and pagination tests in `Identity-client/tests/users/UserManagementComponents.test.jsx`
- [X] T021 [P] [US2] Add authenticated search and paged parsing tests in `Identity-client/tests/users/usersApi.test.js`

### Implementation

- [X] T022 [US2] Remove normalized SQL and use direct Code comparisons in `Identity-api/src/Identity.Infrastructure/Persistence/DapperUsersReadRepository.cs`
- [X] T023 [US2] Verify DTOs expose Code and Email without normalized fields in `Identity-api/src/Identity.Application/Users/Dtos/UsersDto.cs`
- [X] T024 [US2] Preserve permission-gated menu and `/users` restoration in `Identity-client/src/App.jsx`
- [X] T025 [US2] Display Code as the account column in `Identity-client/src/features/users/components/UsersTable.jsx`
- [X] T026 [US2] Integrate Code/name/status filters and server pagination in `Identity-client/src/features/users/UsersPage.jsx`

**Checkpoint**: The read-only Users screen is independently usable.

---

## Phase 5: User Story 3 - Create User (Priority: P1)

**Goal**: Create a User with independent required Code and Email values.

**Independent Test**: Valid input creates a searchable User; invalid/duplicate Code or Email is mapped to its field and password is cleared after submission.

### Tests

- [X] T027 [P] [US3] Add Code ASCII, required Email, email-format, length, and independence tests in `Identity-api/tests/Identity.Application.Tests/Users/UserValidatorsTests.cs`
- [X] T028 [P] [US3] Add create success and field-keyed case-insensitive conflict tests in `Identity-api/tests/Identity.Api.IntegrationTests/Users/UsersControllerContractTests.cs`
- [X] T029 [P] [US3] Add required Email, invalid Code, pending-state, and password-clearing UI tests in `Identity-client/tests/users/UserManagementComponents.test.jsx`

### Implementation

- [X] T030 [US3] Centralize direct Code/Email validation and conflict mapping in `Identity-api/src/Identity.Application/Users/UserIdentityRules.cs`
- [X] T031 [US3] Enforce independent Code and Email contracts in `Identity-api/src/Identity.Application/Users/CreateUsers/CreateUsersValidator.cs`
- [X] T032 [US3] Persist Code and trimmed Email directly without normalized assignments in `Identity-api/src/Identity.Application/Users/CreateUsers/CreateUsersCommandHandler.cs`
- [X] T033 [US3] Send independent Code, Email, name, and password in `Identity-client/src/features/users/api/usersApi.js`
- [X] T034 [US3] Render multiline-formatted MUI Code and required Email fields in `Identity-client/src/features/users/components/UserFormDialog.jsx`

**Checkpoint**: Creation works without normalized properties.

---

## Phase 6: User Story 4 - Update User (Priority: P2)

**Goal**: Update Code, Email, and name independently with optimistic concurrency.

**Independent Test**: Valid edits persist without changing password; duplicates and stale versions do not overwrite data.

### Tests

- [X] T035 [P] [US4] Add required Email, Code-rule, independent-update, and stale-version tests in `Identity-api/tests/Identity.Application.Tests/Users/UserValidatorsTests.cs`
- [X] T036 [P] [US4] Add update success, conflict, not-found, and stale-version API tests in `Identity-api/tests/Identity.Api.IntegrationTests/Users/UsersControllerContractTests.cs`
- [X] T037 [P] [US4] Add populated Code/Email, field-error, and concurrency-recovery UI tests in `Identity-client/tests/users/UserManagementComponents.test.jsx`

### Implementation

- [X] T038 [US4] Enforce independent Code and required Email in `Identity-api/src/Identity.Application/Users/UpdateUsers/UpdateUsersValidator.cs`
- [X] T039 [US4] Persist direct Code/Email with version checks and no normalized assignments in `Identity-api/src/Identity.Application/Users/UpdateUsers/UpdateUsersCommandHandler.cs`
- [X] T040 [US4] Map duplicate and stale responses to form or reload state in `Identity-client/src/features/users/UsersPage.jsx`
- [X] T041 [US4] Keep password out of edit and format MUI fields over multiple lines in `Identity-client/src/features/users/components/UserFormDialog.jsx`

**Checkpoint**: Updates preserve direct identity and concurrency rules.

---

## Phase 7: User Story 5 - Delete User (Priority: P3)

**Goal**: Confirm a soft delete while rejecting self-delete and stale mutations.

**Independent Test**: Confirmed deletion removes the User from active results and blocks login; cancel, self-delete, and stale paths preserve consistent data.

### Tests

- [X] T042 [P] [US5] Add soft-delete, self-delete, stale-version, invalidation, and safe-conflict tests in `Identity-api/tests/Identity.Api.IntegrationTests/Users/UsersControllerContractTests.cs`
- [X] T043 [P] [US5] Add Code/name confirmation, cancel, pending-state, and error tests in `Identity-client/tests/users/UserManagementComponents.test.jsx`

### Implementation

- [X] T044 [US5] Verify soft delete updates lifecycle/version state without normalized access in `Identity-api/src/Identity.Application/Users/DeleteUsers/DeleteUsersCommandHandler.cs`
- [X] T045 [US5] Exclude deleted users from direct reads and authentication queries in `Identity-api/src/Identity.Infrastructure/Persistence/DapperUsersReadRepository.cs`
- [X] T046 [US5] Identify the target by Code and name in `Identity-client/src/features/users/components/DeleteUserDialog.jsx`

**Checkpoint**: All five stories work on the direct Code/Email schema.

---

## Phase 8: Polish and Cross-Cutting Validation

- [X] T047 Review changed C# and JSX against the multiline formatting rule in `.specify/memory/constitution.md`
- [X] T048 Run each backend test project separately and record results in `specs/001-manage-users/quickstart.md`
- [X] T049 Run frontend tests, lint, and production build and record results in `specs/001-manage-users/quickstart.md`
- [X] T050 Execute Code-login and Users CRUD smoke tests against the migrated database in `specs/001-manage-users/quickstart.md`
- [X] T051 Verify rollback recreates/backfills normalized columns and re-apply returns to direct columns in `specs/001-manage-users/quickstart.md`
- [X] T052 Review safe Problem Details and secret-free logging in `Identity-api/src/Identity.Api/Middleware/ExceptionMiddleware.cs`

---

## Dependencies and Execution Order

- Phase 1 has no dependency; Phase 2 depends on Phase 1 and blocks every story.
- US1, US2, US3, and US5 can start after Phase 2; US4 should follow US3 when one owner changes shared validation/form files.
- US5 uses US1 as the proof that deleted accounts cannot authenticate.
- Phase 8 depends on every story selected for release.

```text
Setup -> Foundation -> US1 Login -----------\
                    -> US2 List/Search ------+-> Polish/Release
                    -> US3 Create -> US4 Edit+
                    -> US5 Delete --(verify with US1)
```

## Parallel Opportunities

- T003 can run alongside T002.
- Test tasks within each story can run in parallel.
- After Phase 2, separate owners can implement US1, US2, US3, and US5 concurrently while coordinating shared repository/test files.
- Backend and client tasks touching different files can run in parallel after their story tests exist.

## Implementation Strategy

### Suggested MVP

1. Complete Setup and Foundation.
2. Complete US1 to establish Code login on the new schema.
3. Complete US2 for a read-only Users management slice.
4. Validate both independently before enabling mutations.

### Incremental Delivery

1. Deliver direct-column migration and Code login.
2. Add list/search, then create, update, and delete as separate tested increments.
3. Finish with rollback, security, formatting, and end-to-end gates.

## Notes

- Never add `NormalizedCode` or `NormalizedEmail` back to runtime entities, DTOs, queries, commands, or client contracts.
- Enforce case-insensitive identity with direct column collation and unique indexes, not application-only checks.
- Preserve CQRS: Dapper for reads and EF Core for commands.
- Format C#, JSX, object literals, lambdas, and MUI trees over readable multiple lines per the constitution.

