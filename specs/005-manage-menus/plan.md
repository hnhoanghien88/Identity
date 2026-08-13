# Implementation Plan: Quản lý Menus dạng cây

**Branch**: `005-manage-menus` | **Date**: 2026-08-13 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/005-manage-menus/spec.md`

## Summary

Bổ sung CRUD Menus theo CQRS và màn hình index dạng tree list đệ quy theo `ParentId`. Backend dùng Dapper cho query cây/lookup và EF Core cho commands qua MediatR/FluentValidation; bổ sung `Version` để chống ghi đè đồng thời, kiểm tra quan hệ cùng Application và ngăn chu trình. API trả cây theo Application cùng lookup Applications/Resources động. Frontend React/MUI thêm `/menus`, bộ chọn Application, tree table có expand/collapse và form CRUD accessible.

## Technical Context

**Language/Version**: C# / .NET 10; JavaScript ES modules / React 19  
**Primary Dependencies**: ASP.NET Core, MediatR, FluentValidation, EF Core/MySQL, Dapper; React, Vite, Material UI  
**Storage**: MySQL; bảng `menus`, `applications`, `resources`; thêm `menus.Version`  
**Testing**: xUnit; Vitest + Testing Library; oxlint, Prettier, production build  
**Target Platform**: ASP.NET Core web API và modern desktop/mobile browsers  
**Project Type**: Layered web API and React SPA  
**Performance Goals**: 10.000 Menu/Application, cây sâu 20 cấp; 95% thao tác đọc/expand trong 2 giây  
**Constraints**: CQRS; authorization fail-closed; parameterized SQL; no cycles; same-Application parent/resource; optimistic concurrency; soft delete; không thêm dependency  
**Scale/Scope**: Một màn hình, bốn endpoints chính, lookup dùng endpoints hiện có, một migration, backend/frontend tests

## Constitution Check

*GATE: Passed before research and re-checked after design.*

| Principle / gate | Design evidence | Status |
|---|---|---|
| I. Security and Identity First | Endpoints yêu cầu policy; validate IDs/strings/relations; parameterized SQL; safe errors. | PASS |
| II. Enforced Layer Boundaries | Domain entity; Application use cases/interfaces; Infrastructure persistence; API transport; client API/view. | PASS |
| III. Explicit API Contracts | OpenAPI định nghĩa request, response, auth và 400/401/403/404/409. | PASS |
| IV. Tests Are Release Gates | Unit tests cycle/validation/handlers, persistence and API contracts, frontend interaction/accessibility, all build gates. | PASS |
| V. Consistent and Accessible UX | React/MUI patterns, keyboard/focus, `aria-expanded`, responsive/loading/error states. | PASS |
| VI. Readable Multi-line Code | Prettier/lint và manual multi-line review là task bắt buộc. | PASS |
| Migration governance | Version migration có recovery/rollback và snapshot/schema parity. | PASS |
| Simplicity/scope | Reuse dependencies, repository and UI conventions; no speculative tree library. | PASS |

### Post-design re-check

Data model, OpenAPI và quickstart giữ nguyên mọi gate. Contract giới hạn tree theo Application, các command kiểm tra lại quan hệ tại thời điểm commit, `Version` giải quyết concurrency và soft delete bảo toàn audit. Không có constitution exception.

## Project Structure

```text
specs/005-manage-menus/{plan,research,data-model,quickstart,tasks}.md
specs/005-manage-menus/contracts/menus.openapi.yaml
Identity-api/src/Identity.Application/Menus/
Identity-api/src/Identity.Application/Abstractions/Persistence/{IMenusReadRepository,IMenusRepository}.cs
Identity-api/src/Identity.Infrastructure/Persistence/{DapperMenusReadRepository,MySqlMenusRepository}.cs
Identity-api/src/Identity.Api/{Authorization/MenuPermissionPolicies.cs,Controllers/MenusController.cs}
Identity-api/tests/*/Menus/
Identity-client/src/features/menus/
Identity-client/tests/menus/
```

**Structure Decision**: Giữ backend bốn layer và client theo feature. Menus mirror conventions của Resources nhưng query trả flat rows có `ParentId`; Application handler xây cây có cycle/orphan guard để contract không phụ thuộc SQL đệ quy hay thư viện UI mới.

## Implementation Design

- Giá trị Resource/Route không được thiết lập được giữ là `null` xuyên suốt dữ liệu và form; tree table chỉ ánh xạ giá trị thiếu sang ký hiệu Unicode `—` tại thời điểm render. Nút Edit nhận nguyên bản Menu, không nhận text thay thế, nên bản ghi có cả hai trường `null` vẫn chỉnh sửa được.
- Regression test render một Menu có Resource/Route `null`, xác nhận không có chuỗi mojibake và xác nhận callback Edit nhận đúng object ban đầu.

- Query tree dùng SQL parameterized theo Application, trả toàn bộ rows hoạt động theo thứ tự ổn định; handler dựng cây O(n), cô lập orphan/cycle thành dữ liệu chẩn đoán an toàn.
- Commands dùng EF/MediatR/FluentValidation; repository kiểm tra Application, Resource và Parent cùng scope, descendant cycle, duplicate Code và concurrency.
- `Version` unsigned mặc định 1; update/delete yêu cầu đúng Version và update tăng đúng một lần.
- Soft delete chỉ cho leaf; các read/lookup mặc định loại `IsDeleted`; FK vẫn bảo vệ race cuối.
- API policy `Menus.View/Create/Update/Delete`; response theo `ApiResponse`; lỗi 400/401/403/404/409 theo middleware/conventions hiện có.
- Client tải Applications động, tải tree và Resources theo Application, loại self/descendants khỏi Parent options, giữ dữ liệu form khi retry và cung cấp keyboard tree semantics.

## Complexity Tracking

No constitution violations require justification.

