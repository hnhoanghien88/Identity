# Tasks: Quáº£n lÃ½ Menus dáº¡ng cÃ¢y

**Input**: Design documents from `/specs/005-manage-menus/`  
**Tests**: Required by the project constitution and included before corresponding implementation.

## Phase 1: Setup

- [x] T001 Verify repository ignore/configuration coverage for .NET and Vite artifacts in `.gitignore` and `Identity-client/eslint.config.js`
- [x] T002 Add Menu Version persistence shape and migration recovery notes in `Identity-api/src/Identity.Domain/Entities/MenuTokenEntities.cs`, `Identity-api/src/Identity.Infrastructure/Persistence/Configurations/SchemaConfigurations.cs`, and `Identity-api/src/Identity.Infrastructure/Persistence/Sql/identity_db.sql`

## Phase 2: Foundational

- [x] T003 [P] Add Menu permissions constants in `Identity-api/src/Identity.Application/Common/Authorization/MenuPermissions.cs`
- [x] T004 [P] Define Menu read/write repository contracts in `Identity-api/src/Identity.Application/Abstractions/Persistence/IMenusReadRepository.cs` and `Identity-api/src/Identity.Application/Abstractions/Persistence/IMenusRepository.cs`
- [x] T005 [P] Define Menu DTOs and shared validation rules in `Identity-api/src/Identity.Application/Menus/Dtos/MenuDto.cs` and `Identity-api/src/Identity.Application/Menus/MenuRules.cs`
- [x] T006 Register Menu repositories and policy infrastructure in `Identity-api/src/Identity.Infrastructure/DependencyInjection.cs`, `Identity-api/src/Identity.Api/Authorization/MenuPermissionPolicies.cs`, and `Identity-api/src/Identity.Api/Program.cs`
- [x] T007 Create and align Menu Version migration, snapshot, and schema in `Identity-api/src/Identity.Infrastructure/Migrations/`, `Identity-api/src/Identity.Infrastructure/Migrations/IdentityDbContextModelSnapshot.cs`, and `Identity-api/src/Identity.Infrastructure/Persistence/Sql/identity_db.sql`

## Phase 3: User Story 1 - Xem vÃ  duyá»‡t cÃ¢y Menu (P1)

**Goal**: Hiá»ƒn thá»‹ cÃ¢y Menu Ä‘Ãºng Application, quan há»‡, thá»© tá»± vÃ  tráº¡ng thÃ¡i expand/collapse.  
**Independent Test**: Seed cÃ¢y ba cáº¥p, má»Ÿ `/menus`, kiá»ƒm tra Ä‘Ãºng parent/order/application vÃ  thao tÃ¡c tá»«ng nhÃ¡nh/toÃ n cÃ¢y.

- [x] T008 [P] [US1] Add query/tree assembly tests including orphan and cycle guards in `Identity-api/tests/Identity.Application.Tests/Menus/GetMenusTests.cs`
- [x] T009 [P] [US1] Add API authorization and response contract tests in `Identity-api/tests/Identity.Api.IntegrationTests/Menus/MenusReadContractTests.cs`
- [x] T010 [P] [US1] Add frontend tree interaction and accessibility tests in `Identity-client/tests/menus/menus-tree.test.jsx`
- [x] T011 [US1] Implement parameterized flat Menu reads in `Identity-api/src/Identity.Infrastructure/Persistence/DapperMenusReadRepository.cs`
- [x] T012 [US1] Implement GetMenus/GetMenuById CQRS queries and guarded tree assembly in `Identity-api/src/Identity.Application/Menus/GetMenus/` and `Identity-api/src/Identity.Application/Menus/GetMenuById/`
- [x] T013 [US1] Expose authorized Menu reads in `Identity-api/src/Identity.Api/Controllers/MenusController.cs`
- [x] T014 [US1] Implement Menu client API, capabilities and recursive tree table in `Identity-client/src/features/menus/api/`, `Identity-client/src/features/menus/capabilities.js`, and `Identity-client/src/features/menus/components/MenusTreeTable.jsx`
- [x] T015 [US1] Integrate Application selector, loading/empty/error states, route and navigation in `Identity-client/src/features/menus/MenusPage.jsx`, `Identity-client/src/features/menus/index.js`, and `Identity-client/src/App.jsx`

## Phase 4: User Story 2 - Táº¡o Menu táº¡i vá»‹ trÃ­ há»£p lá»‡ (P1)

**Goal**: Táº¡o root/child Menu vá»›i Application, Resource vÃ  Parent láº¥y Ä‘á»™ng.  
**Independent Test**: Táº¡o root vÃ  child, reload, kiá»ƒm tra scope/resource/order; invalid cross-scope vÃ  duplicate bá»‹ tá»« chá»‘i.

- [ ] T016 [P] [US2] Add create validation and handler tests in `Identity-api/tests/Identity.Application.Tests/Menus/CreateMenuTests.cs`
- [ ] T017 [P] [US2] Add create persistence/contract tests in `Identity-api/tests/Identity.Infrastructure.IntegrationTests/Menus/MenuCreatePersistenceTests.cs` and `Identity-api/tests/Identity.Api.IntegrationTests/Menus/MenusCreateContractTests.cs`
- [ ] T018 [P] [US2] Add dynamic lookup and create form tests in `Identity-client/tests/menus/menu-create.test.jsx`
- [x] T019 [US2] Implement create persistence with scope and uniqueness checks in `Identity-api/src/Identity.Infrastructure/Persistence/MySqlMenusRepository.cs`
- [x] T020 [US2] Implement CreateMenu command, validator and handler in `Identity-api/src/Identity.Application/Menus/CreateMenu/`
- [x] T021 [US2] Expose create endpoint in `Identity-api/src/Identity.Api/Controllers/MenusController.cs`
- [x] T022 [US2] Implement dynamic Application/Resource/Parent form and create flow in `Identity-client/src/features/menus/components/MenuFormDialog.jsx` and `Identity-client/src/features/menus/MenusPage.jsx`

## Phase 5: User Story 3 - Cáº­p nháº­t vÃ  di chuyá»ƒn Menu (P2)

**Goal**: Sá»­a hoáº·c move cáº£ subtree, cháº·n cycle/cross-scope/stale writes.  
**Independent Test**: Move má»™t node cÃ³ descendant, rá»“i thá»­ self/descendant parent, Ä‘á»•i Application vÃ  stale Version.

- [ ] T023 [P] [US3] Add update, move, cycle and concurrency tests in `Identity-api/tests/Identity.Application.Tests/Menus/UpdateMenuTests.cs`
- [ ] T024 [P] [US3] Add update persistence/API contract tests in `Identity-api/tests/Identity.Infrastructure.IntegrationTests/Menus/MenuUpdatePersistenceTests.cs` and `Identity-api/tests/Identity.Api.IntegrationTests/Menus/MenusUpdateContractTests.cs`
- [ ] T025 [P] [US3] Add edit/move and stale response frontend tests in `Identity-client/tests/menus/menu-update.test.jsx`
- [x] T026 [US3] Implement update/move persistence with ancestor and Version checks in `Identity-api/src/Identity.Infrastructure/Persistence/MySqlMenusRepository.cs`
- [x] T027 [US3] Implement UpdateMenu command, validator and handler in `Identity-api/src/Identity.Application/Menus/UpdateMenu/`
- [x] T028 [US3] Expose update endpoint in `Identity-api/src/Identity.Api/Controllers/MenusController.cs`
- [x] T029 [US3] Implement edit/move UI with descendant exclusions and conflict recovery in `Identity-client/src/features/menus/components/MenuFormDialog.jsx` and `Identity-client/src/features/menus/MenusPage.jsx`

## Phase 6: User Story 4 - XÃ³a Menu an toÃ n (P3)

**Goal**: Soft-delete leaf vÃ  cháº·n parent/concurrent delete.  
**Independent Test**: Delete leaf thÃ nh cÃ´ng; delete parent, cancel vÃ  stale Version giá»¯ nguyÃªn tree.

- [ ] T030 [P] [US4] Add delete leaf/dependency/concurrency handler tests in `Identity-api/tests/Identity.Application.Tests/Menus/DeleteMenuTests.cs`
- [ ] T031 [P] [US4] Add soft-delete persistence/API contract tests in `Identity-api/tests/Identity.Infrastructure.IntegrationTests/Menus/MenuDeletePersistenceTests.cs` and `Identity-api/tests/Identity.Api.IntegrationTests/Menus/MenusDeleteContractTests.cs`
- [ ] T032 [P] [US4] Add delete confirmation and blocked-result frontend tests in `Identity-client/tests/menus/menu-delete.test.jsx`
- [x] T033 [US4] Implement leaf-only soft-delete persistence in `Identity-api/src/Identity.Infrastructure/Persistence/MySqlMenusRepository.cs`
- [x] T034 [US4] Implement DeleteMenu command and handler in `Identity-api/src/Identity.Application/Menus/DeleteMenu/`
- [x] T035 [US4] Expose delete endpoint in `Identity-api/src/Identity.Api/Controllers/MenusController.cs`
- [x] T036 [US4] Implement delete confirmation and tree refresh in `Identity-client/src/features/menus/components/DeleteMenuDialog.jsx` and `Identity-client/src/features/menus/MenusPage.jsx`

## Phase 7: Polish & Cross-Cutting Concerns

- [ ] T037 [P] Add Menu read scale and migration coverage in `Identity-api/tests/Identity.Infrastructure.IntegrationTests/Menus/MenuPerformanceTests.cs` and `Identity-api/tests/Identity.Infrastructure.IntegrationTests/Menus/MenuMigrationTests.cs`
- [x] T038 [P] Update runnable validation documentation in `specs/005-manage-menus/quickstart.md` and API examples in `Identity-api/src/Identity.Api/Identity.Api.http`
- [x] T039 Run formatter and manually review multi-line JSX/object formatting in `Identity-client/src/features/menus/` and `Identity-client/tests/menus/`
- [ ] T040 Run backend build/tests and frontend test/lint/build gates from `specs/005-manage-menus/quickstart.md`

## Phase 8: Resource/Route NULL Regression (US3)

**Goal**: Hiển thị đúng giá trị thiếu và giữ thao tác chỉnh sửa hoạt động khi Resource/Route là `NULL`.
**Independent Test**: Render Menu có `resourceName` và `route` trống, xác nhận hai ô hiển thị `—`, không có mojibake, và nút Edit truyền đúng Menu gốc.

- [x] T041 [P] [US3] Add NULL Resource/Route display and edit regression coverage in `Identity-client/tests/menus/menus-tree.test.jsx`
- [x] T042 [US3] Render the Unicode empty-value placeholder without mutating edit data in `Identity-client/src/features/menus/components/MenusTreeTable.jsx`

## Phase 9: Editable Application and Scoped Resource Lookups (US3)

**Goal**: Cho phép chọn Application trong form tạo/chỉnh sửa và luôn tải Parent/Resource theo Application của form.
**Independent Test**: Mở form sửa Menu, đổi Application, xác nhận Parent/Resource cũ bị xóa, lookup mới chỉ thuộc Application vừa chọn; lưu Menu lá thành công và từ chối Menu còn con.

- [x] T043 [P] [US3] Add update handler coverage for leaf and non-leaf Application changes in `Identity-api/tests/Identity.Application.Tests/Menus/UpdateMenuTests.cs`
- [x] T044 [P] [US3] Add form Application selection and scoped Resource reload coverage in `Identity-client/tests/menus/menu-update.test.jsx` and `Identity-client/tests/menus/menus-page.test.jsx`
- [x] T045 [US3] Permit validated leaf Menu Application changes in `Identity-api/src/Identity.Application/Menus/UpdateMenu/UpdateMenuCommandHandler.cs`
- [x] T046 [US3] Add Application selection and dependent-field reset behavior in `Identity-client/src/features/menus/components/MenuFormDialog.jsx`
- [x] T047 [US3] Load form Menu/Resource lookups by the form-selected Application in `Identity-client/src/features/menus/MenusPage.jsx`
- [x] T048 Update design contracts and runnable validation in `specs/005-manage-menus/plan.md`, `specs/005-manage-menus/research.md`, `specs/005-manage-menus/data-model.md`, `specs/005-manage-menus/contracts/menus.openapi.yaml`, and `specs/005-manage-menus/quickstart.md`
- [x] T049 Run targeted backend/frontend tests plus formatter, lint, and build gates from `specs/005-manage-menus/quickstart.md`

## Phase 10: Inline Order Commit (US3)

**Goal**: Lưu Order bằng Enter hoặc blur, chống gửi trùng và cập nhật đúng dòng tại chỗ mà không tải lại tree.
**Independent Test**: Sửa Order rồi blur để xác nhận API gọi một lần; sửa lại, nhấn Enter rồi blur để xác nhận không gửi trùng, node/version cập nhật tại chỗ và `getMenus` không gọi lại.

- [x] T050 [P] [US3] Add Enter, blur, deduplication, and Escape regression tests in `Identity-client/tests/menus/menus-tree.test.jsx`
- [x] T051 [P] [US3] Add no-refetch inline Order integration coverage in `Identity-client/tests/menus/menus-page.test.jsx`
- [x] T052 [US3] Implement Enter-or-blur commit with duplicate-submit protection in `Identity-client/src/features/menus/components/MenusTreeTable.jsx`
- [x] T053 [US3] Merge saved Order and Version into the existing tree without reload in `Identity-client/src/features/menus/MenusPage.jsx`
- [x] T054 Update inline Order design and validation guidance in `specs/005-manage-menus/spec.md`, `specs/005-manage-menus/plan.md`, `specs/005-manage-menus/research.md`, and `specs/005-manage-menus/quickstart.md`
- [x] T055 Run targeted Menu tests, formatter, lint, and production build for the inline Order change
## Dependencies

- Phase 1 â†’ Phase 2 â†’ user stories â†’ Polish.
- US1 establishes read/tree/navigation used by US2â€“US4.
- US2 depends on foundational persistence and US1 refresh behavior.
- US3 depends on US2 form/repository; US4 depends on US1 tree and shared repository.
- Within each story, tests precede implementation; tasks touching the same file execute sequentially.
- T041 precedes T042; the NULL regression reuses the completed US1 tree and US3 edit surfaces.
- T043 and T044 precede T045-T047; T045 and T047 can proceed independently before integrated validation.
- T050 and T051 precede T052-T053; local merge validation follows Enter-only cell behavior.

## Parallel Opportunities

- T003â€“T005 can run in parallel; T008â€“T010, T016â€“T018, T023â€“T025 and T030â€“T032 are parallel test surfaces.
- After backend contracts stabilize, backend and frontend work in each story can progress independently until integration.
- T037 and T038 can run in parallel after all stories.

## Implementation Strategy

1. Complete foundation and US1 as the MVP: authorized, resilient read-only Menu tree.
2. Add create with dynamic lookups, then update/move, then safe delete.
3. Run each story's targeted tests before advancing; finish with complete release gates.


