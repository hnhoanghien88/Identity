# Implementation Plan: Quản lý Resources

**Branch**: `003-manage-resources` | **Date**: 2026-08-11 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/003-manage-resources/spec.md`

## Summary

Bổ sung màn hình CRUD Resources có lọc, sắp xếp, phân trang và chọn Application. Backend áp dụng CQRS theo pattern Applications: MediatR vertical slices, Dapper read repository cho search/detail và EF Core write repository cho create/update/soft-delete. Bổ sung optimistic concurrency `Version` cho `resources`, audit mutation, kiểm tra Application hợp lệ, bảo vệ Permission/Menu trước soft-delete, bốn permission policy độc lập và API contract nhất quán. Frontend tạo feature Resources bằng React/MUI, giữ dữ liệu cũ khi reload lỗi và chỉ hiển thị hành động theo capability.

## Technical Context

**Language/Version**: C# / .NET 10; JavaScript ES modules / React 19  
**Primary Dependencies**: ASP.NET Core, MediatR 14.2, FluentValidation 12.1, EF Core/MySql.EntityFrameworkCore 10.0.7, Dapper 2.1; React 19, Vite 8, Material UI 9  
**Storage**: MySQL; bảng `resources` hiện có, thêm `Version` và bảo đảm uniqueness không phân biệt hoa/thường cho `(ApplicationId, Code)`  
**Testing**: xUnit backend unit/integration/contract; Vitest + Testing Library/jsdom frontend; oxlint, Prettier và build gates  
**Target Platform**: ASP.NET Core web API và trình duyệt desktop/mobile hiện đại được client hỗ trợ  
**Project Type**: Web application với backend phân lớp và React SPA  
**Performance Goals**: Với tối đa 10.000 Resources, 95% thao tác list/filter/sort/page hiển thị trong 2 giây dưới tải bình thường  
**Constraints**: CQRS; authorization fail-closed; parameterized SQL; không truy cập DB từ client; soft-delete; optimistic concurrency; bảo vệ dependency; không thêm dependency  
**Scale/Scope**: Một màn hình, năm endpoints, bốn permission policies, một schema migration, backend/frontend tests

## Constitution Check

*GATE: Passed before research and re-checked after design.*

| Principle / gate | Design evidence | Status |
|---|---|---|
| I. Security and Identity First | Named policies trên mọi endpoint; validation trước persistence; parameterized SQL; lỗi an toàn. | PASS |
| II. Enforced Layer Boundaries | Domain entity; Application use cases/interfaces; Infrastructure Dapper/EF; API transport/auth; client view/API tách biệt. | PASS |
| III. Explicit API Contracts | OpenAPI định nghĩa request, response, auth và errors; client xử lý HTTP outcomes thực. | PASS |
| IV. Tests Are Release Gates | Unit, persistence, API auth/contract và frontend tests cùng lệnh validation trong quickstart. | PASS |
| V. Consistent and Accessible UX | Tái sử dụng MUI/shared conventions với keyboard, focus, responsive và state feedback. | PASS |
| VI. Readable Multi-line Code | Prettier/lint và manual formatting review là release gate. | PASS |
| Migration governance | Migration Version/collation có validation trên DB đại diện và recovery guidance. | PASS |
| Simplicity/scope | Tái sử dụng projects, patterns và dependencies hiện có; không refactor ngoài phạm vi. | PASS |

### Post-design re-check

Data model, OpenAPI và quickstart duy trì mọi gate. `Version` đóng concurrency gap; policy độc lập bảo vệ direct API; dependency checks bảo toàn Permission/Menu; migration có rollback/recovery. Không cần constitution exception.

## Project Structure

### Documentation (this feature)

```text
specs/003-manage-resources/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── resources.openapi.yaml
└── tasks.md                         # Created later by $speckit-tasks
```

### Source Code (repository root)

```text
Identity-api/
├── src/
│   ├── Identity.Domain/Entities/AuthorizationEntities.cs
│   ├── Identity.Application/
│   │   ├── Abstractions/Persistence/{IResourcesReadRepository,IResourcesRepository}.cs
│   │   ├── Resources/{Dtos,GetResources,GetResourceById,CreateResource,UpdateResource,DeleteResource}/
│   │   └── Common/Authorization/ResourcePermissions.cs
│   ├── Identity.Infrastructure/
│   │   ├── Persistence/{DapperResourcesReadRepository,MySqlResourcesRepository}.cs
│   │   ├── Persistence/Configurations/SchemaConfigurations.cs
│   │   └── Migrations/
│   └── Identity.Api/
│       ├── Controllers/ResourcesController.cs
│       ├── Authorization/
│       └── Program.cs
└── tests/
    ├── Identity.Application.Tests/Resources/
    ├── Identity.Infrastructure.IntegrationTests/Resources/
    └── Identity.Api.IntegrationTests/Resources/

Identity-client/
├── src/
│   ├── App.jsx
│   └── features/resources/
│       ├── ResourcesPage.jsx
│       ├── index.js
│       ├── capabilities.js
│       ├── api/{resourcesApi,resourcesClient}.js
│       └── components/{ResourcesFilters,ResourcesTable,ResourceFormDialog,DeleteResourceDialog}.jsx
└── tests/resources/
```

**Structure Decision**: Giữ backend bốn layer và client theo feature. Mirror Applications để giảm rủi ro, nhưng thêm lookup Application, permission policies và dependency/concurrency guarantees riêng của Resources.

## Implementation Design

### CQRS and persistence

- `GetResources` và `GetResourceById` dùng `IResourcesReadRepository`; Dapper project DTO cùng Application Code/Name, parameterize filters, allow-list sorts và loại deleted rows.
- Create/Update/Delete dùng `IResourcesRepository`; EF Core thực hiện transactional writes và chuyển unique/concurrency/dependency failures thành safe exceptions.
- Create xác minh Application active/non-deleted, trim fields, mặc định active/not-deleted/Version 1 và stamp actor/time.
- Update yêu cầu Version hiện tại; có thể đổi Application nếu đích hợp lệ; cập nhật editable fields, tăng Version đúng một lần và stamp audit.
- Delete yêu cầu Version, kiểm tra Permissions/Menus trong cùng command transaction rồi đặt deleted/inactive, tăng Version và stamp audit.
- Unique key gồm deleted rows để khớp index hiện tại. Migration thêm Version và làm collation Code rõ ràng, case-insensitive.

### Authorization and errors

- Dùng policies `Resources.View`, `Resources.Create`, `Resources.Update`, `Resources.Delete`; server luôn là nguồn quyết định.
- Search/detail yêu cầu View; create/update/delete yêu cầu policy tương ứng. Client dùng claims chỉ để điều chỉnh UX.
- Success dùng `ApiResponse<T>`; validation 400, auth 401/403, absent/deleted 404, duplicate/stale/dependency 409 và safe 500 dùng Problem Details.

### Client behavior

- Thêm `/resources` và menu Resources; bảo vệ direct route bằng capability View.
- Form dùng Application selector chỉ gồm active/non-deleted Applications; edit vẫn hiển thị Application hiện tại và tải lại khi stale.
- State gồm draft/applied filters, paging/sorting, rows cũ, loading/error/retry, dialogs, pending mutation và notices.
- Reload lỗi giữ rows cũ; mutation 409 giữ form/context. Dialog có focus trap/restore, field errors, disabled submit và responsive width.

### Migration and operational risk

- Migration thêm `Version` NOT NULL default 1 và cấu hình concurrency token.
- Kiểm tra trước case-only duplicates trong từng Application; migration phải dừng an toàn nếu có conflict trước khi áp collation/index.
- Rollback gỡ Version và phục hồi metadata Code chỉ sau khi xác nhận không có client mới phụ thuộc; dùng backup và coordinated application rollback.
- Không đổi foreign-key ownership; delete protection là application-level vì soft-delete không kích hoạt FK restrict.

## Complexity Tracking

No constitution violations require justification.
