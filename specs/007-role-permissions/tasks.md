# Tasks: PhÃƒÂ¢n quyÃ¡Â»Ân theo Role

**Input**: Design documents from `/specs/007-role-permissions/`

## Phase 1: Setup

- [X] T001 Create RolePermissions feature and test directories under Identity-api/src, Identity-api/tests, Identity-client/src/features/rolePermissions and Identity-client/tests/rolePermissions

## Phase 2: Foundational

- [X] T002 [P] Define Role Permission DTOs, queries and commands in Identity-api/src/Identity.Application/RolePermissions
- [X] T003 [P] Define IRolePermissionsRepository contract in Identity-api/src/Identity.Application/Abstractions/Persistence/IRolePermissionsRepository.cs
- [X] T004 Register Role Permissions persistence in Identity-api/src/Identity.Infrastructure/DependencyInjection.cs

## Phase 3: User Story 1 - ChÃ¡Â»Ân Role vÃƒÂ  Resource Ã„â€˜Ã¡Â»Æ’ xem quyÃ¡Â»Ân (P1) MVP

**Goal**: NgÃ†Â°Ã¡Â»Âi dÃƒÂ¹ng Ã„â€˜ÃƒÂ£ Ã„â€˜Ã„Æ’ng nhÃ¡ÂºÂ­p chÃ¡Â»Ân Role vÃƒÂ  Resource trong giao diÃ¡Â»â€¡n ba cÃ¡Â»â„¢t Ã„â€˜Ã¡Â»Æ’ xem Ã„â€˜ÃƒÂºng trÃ¡ÂºÂ¡ng thÃƒÂ¡i cÃ¡Â»Â§a mÃ¡Â»Âi Action.

**Independent Test**: MÃ¡Â»Å¸ `/role-permissions` khÃƒÂ´ng cÃƒÂ³ claim riÃƒÂªng, xÃƒÂ¡c nhÃ¡ÂºÂ­n lÃ¡Â»Â±a chÃ¡Â»Ân mÃ¡ÂºÂ·c Ã„â€˜Ã¡Â»â€¹nh, Ã„â€˜Ã¡Â»â€¢i Role/Resource nhanh vÃƒÂ  kiÃ¡Â»Æ’m tra chÃ¡Â»â€° snapshot mÃ¡Â»â€ºi nhÃ¡ÂºÂ¥t Ã„â€˜Ã†Â°Ã¡Â»Â£c hiÃ¡Â»Æ’n thÃ¡Â»â€¹; anonymous API nhÃ¡ÂºÂ­n 401.

- [X] T005 [P] [US1] Add snapshot query handler and validation tests in Identity-api/tests/Identity.Application.Tests/RolePermissions/RolePermissionHandlersTests.cs
- [X] T006 [P] [US1] Add authenticated snapshot API contract tests in Identity-api/tests/Identity.Api.IntegrationTests/RolePermissions/RolePermissionsControllerTests.cs
- [X] T007 [P] [US1] Add three-column selection, stale-response and accessibility tests in Identity-client/tests/rolePermissions/RolePermissionsPage.test.jsx
- [X] T008 [US1] Implement snapshot read and reference validation in Identity-api/src/Identity.Infrastructure/Persistence/MySqlRolePermissionsRepository.cs and Identity-api/src/Identity.Application/RolePermissions
- [X] T009 [US1] Implement authenticated GET endpoint in Identity-api/src/Identity.Api/Controllers/RolePermissionsController.cs
- [X] T010 [US1] Implement API client, three-column components and RolePermissionsPage in Identity-client/src/features/rolePermissions
- [X] T011 [US1] Add authenticated `/role-permissions` route and navigation in Identity-client/src/App.jsx and responsive layout styles in Identity-client/src/App.css

## Phase 4: User Story 2 - CÃ¡ÂºÂ¥p Permission bÃ¡ÂºÂ±ng checkbox (P1)

**Goal**: Check mÃ¡Â»â„¢t Action tÃ¡ÂºÂ¡o idempotently Permission vÃƒÂ  RolePermission Ã„â€˜ÃƒÂºng cho lÃ¡Â»Â±a chÃ¡Â»Ân active.

**Independent Test**: CÃ¡ÂºÂ¥p mÃ¡Â»â„¢t Action chÃ†Â°a cÃƒÂ³, retry cÃƒÂ¹ng request vÃƒÂ  tÃ¡ÂºÂ£i lÃ¡ÂºÂ¡i; checkbox vÃ¡ÂºÂ«n check vÃƒÂ  chÃ¡Â»â€° tÃ¡Â»â€œn tÃ¡ÂºÂ¡i mÃ¡Â»â„¢t liÃƒÂªn kÃ¡ÂºÂ¿t Ã¡Â»Å¸ mÃ¡Â»â€”i unique boundary.

- [X] T012 [P] [US2] Add grant idempotency, transaction, missing-reference and API contract tests in Identity-api/tests/Identity.Application.Tests/RolePermissions/RolePermissionHandlersTests.cs and Identity-api/tests/Identity.Api.IntegrationTests/RolePermissions/RolePermissionsControllerTests.cs
- [X] T013 [US2] Implement transactional idempotent grant in Identity-api/src/Identity.Infrastructure/Persistence/MySqlRolePermissionsRepository.cs and Identity-api/src/Identity.Application/RolePermissions
- [X] T014 [US2] Implement PUT grant endpoint in Identity-api/src/Identity.Api/Controllers/RolePermissionsController.cs and optimistic checkbox grant with rollback in Identity-client/src/features/rolePermissions

## Phase 5: User Story 3 - GÃ¡Â»Â¡ Permission bÃ¡ÂºÂ±ng checkbox (P1)

**Goal**: BÃ¡Â»Â check xÃƒÂ³a idempotently RolePermission nhÃ†Â°ng giÃ¡Â»Â¯ Permission dÃƒÂ¹ng chung.

**Independent Test**: GÃ¡Â»Â¡ mÃ¡Â»â„¢t Action Ã„â€˜ÃƒÂ£ cÃ¡ÂºÂ¥p, retry vÃƒÂ  tÃ¡ÂºÂ£i lÃ¡ÂºÂ¡i; checkbox khÃƒÂ´ng check, RolePermission khÃƒÂ´ng cÃƒÂ²n vÃƒÂ  Permission ResourceÃ¢â‚¬â€œAction vÃ¡ÂºÂ«n tÃ¡Â»â€œn tÃ¡ÂºÂ¡i.

- [X] T015 [P] [US3] Add revoke idempotency, physical Permission deletion and dependency conflict and rollback tests in Identity-api/tests/Identity.Application.Tests/RolePermissions/RolePermissionHandlersTests.cs, Identity-api/tests/Identity.Api.IntegrationTests/RolePermissions/RolePermissionsControllerTests.cs and Identity-client/tests/rolePermissions/RolePermissionsPage.test.jsx
- [X] T016 [US3] Implement idempotent revoke in Identity-api/src/Identity.Infrastructure/Persistence/MySqlRolePermissionsRepository.cs and Identity-api/src/Identity.Application/RolePermissions
- [X] T017 [US3] Implement DELETE revoke endpoint in Identity-api/src/Identity.Api/Controllers/RolePermissionsController.cs and checkbox revoke/rollback UX in Identity-client/src/features/rolePermissions

## Phase 6: Polish & Cross-Cutting Concerns

- [X] T018 Run backend build/tests and frontend formatter/lint/tests/build, validate specs/007-role-permissions/quickstart.md, and review changed JSX/C# for readable multi-line formatting

## Dependencies

## Phase 7: Application-scoped Role and Resource update

- [X] T019 [P] [US4] Update specification and design artifacts in specs/007-role-permissions/ for Role Application display and Resource scoping
- [X] T020 [P] [US4] Add frontend regression coverage in Identity-client/tests/rolePermissions/RolePermissionsPage.test.jsx
- [X] T021 [US4] Display Role Application and reload Resources by selected Role Application in Identity-client/src/features/rolePermissions/RolePermissionsPage.jsx
- [X] T022 [US4] Enforce Role–Resource Application equality in Identity-api/src/Identity.Infrastructure/Persistence/MySqlRolePermissionsRepository.cs
- [X] T023 [US4] Run backend and frontend quality gates and record completion in specs/007-role-permissions/tasks.md

- T001 Ã¢â€ â€™ T002-T004 Ã¢â€ â€™ US1 Ã¢â€ â€™ US2 Ã¢â€ â€™ US3 Ã¢â€ â€™ T018.
- T002 vÃƒÂ  T003 Ã„â€˜Ã¡Â»â„¢c lÃ¡ÂºÂ­p sau setup; T004 phÃ¡Â»Â¥ thuÃ¡Â»â„¢c interface Ã¡Â»Å¸ T003.
- US1 cung cÃ¡ÂºÂ¥p snapshot, controller vÃƒÂ  UI nÃ¡Â»Ân; US2/US3 bÃ¡Â»â€¢ sung hai transition Ã„â€˜Ã¡Â»â„¢c lÃ¡ÂºÂ­p trÃƒÂªn cÃƒÂ¹ng checkbox.
- Test tasks cÃƒÂ³ thÃ¡Â»Æ’ chuÃ¡ÂºÂ©n bÃ¡Â»â€¹ song song theo marker `[P]`, nhÃ†Â°ng implementation cÃƒÂ¹ng repository/controller/UI phÃ¡ÂºÂ£i tuÃ¡ÂºÂ§n tÃ¡Â»Â±.

## Parallel Execution Examples

- Foundation: T002 vÃƒÂ  T003 cÃƒÂ³ thÃ¡Â»Æ’ thÃ¡Â»Â±c hiÃ¡Â»â€¡n song song.
- US1: T005, T006 vÃƒÂ  T007 cÃƒÂ³ thÃ¡Â»Æ’ viÃ¡ÂºÂ¿t song song trÃ†Â°Ã¡Â»â€ºc T008-T011.
- US2/US3: backend application/API cases vÃƒÂ  frontend cases trong mÃ¡Â»â€”i task test cÃƒÂ³ thÃ¡Â»Æ’ chuÃ¡ÂºÂ©n bÃ¡Â»â€¹ Ã„â€˜Ã¡Â»â„¢c lÃ¡ÂºÂ­p trÃ†Â°Ã¡Â»â€ºc implementation.

## Implementation Strategy

MVP gÃ¡Â»â€œm Phase 1-3: xem snapshot Ã„â€˜ÃƒÂºng bÃ¡ÂºÂ±ng giao diÃ¡Â»â€¡n ba cÃ¡Â»â„¢t. Sau Ã„â€˜ÃƒÂ³ cÃ¡ÂºÂ¥p vÃƒÂ  gÃ¡Â»Â¡ quyÃ¡Â»Ân theo hai increment, mÃ¡Â»â€”i increment chÃ¡ÂºÂ¡y independent test trÃ†Â°Ã¡Â»â€ºc khi tiÃ¡ÂºÂ¿p tÃ¡Â»Â¥c. TÃ¡ÂºÂ¥t cÃ¡ÂºÂ£ 18 tasks dÃƒÂ¹ng Ã„â€˜ÃƒÂºng checkbox, ID, marker story vÃƒÂ  Ã„â€˜Ã†Â°Ã¡Â»Âng dÃ¡ÂºÂ«n cÃ¡Â»Â¥ thÃ¡Â»Æ’.
