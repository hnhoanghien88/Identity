# Implementation Plan: Quản lý Actions

**Branch**: `004-manage-actions` | **Date**: 2026-08-12 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/004-manage-actions/spec.md`

## Summary

Bổ sung màn hình CRUD Actions có lọc, sắp xếp và phân trang, lưu vào `permission_actions`. Backend mirror CQRS vertical slices của Applications: Dapper cho read, EF Core cho write, MediatR/FluentValidation cho use cases; thêm `Version` để chống ghi đè đồng thời, xóa vật lý có kiểm tra Permission phụ thuộc. API chỉ yêu cầu phiên đăng nhập và không định nghĩa policy Actions.*. Frontend React/MUI thêm `/actions`, menu và trải nghiệm CRUD nhất quán với Applications.

## Technical Context

**Language/Version**: C# / .NET 10; JavaScript ES modules / React 19  
**Primary Dependencies**: ASP.NET Core, MediatR, FluentValidation, EF Core/MySQL, Dapper; React, Vite, Material UI  
**Storage**: MySQL; bảng `permission_actions`, thêm `Version`, Code unique case-insensitive  
**Testing**: xUnit; Vitest + Testing Library; oxlint, Prettier, production build  
**Target Platform**: ASP.NET Core web API và modern desktop/mobile browsers  
**Project Type**: Layered web API and React SPA  
**Performance Goals**: 10.000 Actions, 95% list/filter/sort/page trong 2 giây  
**Constraints**: CQRS; authenticated-only; không Actions policies; parameterized SQL; concurrency; dependency-safe physical delete; không thêm dependency  
**Scale/Scope**: Một màn hình, năm endpoints, một migration, backend/frontend tests

## Constitution Check

*GATE: Passed before research and re-checked after design.*

| Principle / gate | Design evidence | Status |
|---|---|---|
| I. Security and Identity First | Endpoints yêu cầu xác thực; validation, parameterized SQL, safe errors. | PASS |
| II. Enforced Layer Boundaries | Domain, Application use cases/interfaces, Infrastructure persistence, API transport, client API/view. | PASS |
| III. Explicit API Contracts | OpenAPI định nghĩa request, response, auth và errors. | PASS |
| IV. Tests Are Release Gates | Unit, persistence, API auth/contract và frontend tests; build/lint gates. | PASS |
| V. Consistent and Accessible UX | React/MUI conventions, keyboard/focus/responsive/loading/error. | PASS |
| VI. Readable Multi-line Code | Prettier/lint và manual formatting review. | PASS |
| Migration governance | Version/collation migration có duplicate precheck và recovery. | PASS |
| Simplicity/scope | Tái sử dụng patterns/dependencies; không authorization model mới. | PASS |

### Post-design re-check

Data model, OpenAPI và quickstart duy trì mọi gate. `[Authorize]` bảo vệ API mà không trái yêu cầu không phân quyền; Version đóng concurrency gap; Permission check bảo vệ integrity trước physical delete. Không cần exception.

## Project Structure

```text
specs/004-manage-actions/{plan,research,data-model,quickstart,tasks}.md
specs/004-manage-actions/contracts/actions.openapi.yaml
Identity-api/src/Identity.Application/Actions/
Identity-api/src/Identity.Application/Abstractions/Persistence/{IActionsReadRepository,IActionsRepository}.cs
Identity-api/src/Identity.Infrastructure/Persistence/{DapperActionsReadRepository,MySqlActionsRepository}.cs
Identity-api/src/Identity.Api/Controllers/ActionsController.cs
Identity-api/tests/*/Actions/
Identity-client/src/features/actions/
Identity-client/tests/actions/
```

**Structure Decision**: Giữ backend bốn layer và client theo feature, mirror Applications nhưng bỏ capability/policy/status và dùng dependency-safe physical delete.

## Implementation Design

- Queries dùng Dapper parameterized filters, allow-listed sorts và paging; commands dùng EF/MediatR/FluentValidation.
- Thêm unsigned `Version` default 1; update/delete bắt buộc matching Version; update increment một lần.
- Delete kiểm tra `permissions.ActionId`, xóa vật lý; FK bảo vệ race cuối.
- Controller `[Authorize]`; không Actions policies/capability gates. ApiResponse success; 400/401/404/409 Problem Details.
- Client thêm `/actions`, menu cho session hợp lệ; form Code/Name; retained rows, retry, keyboard/accessibility như Applications.
- Migration precheck duplicates, explicit case-insensitive Code collation, Version; rollback cần backup và coordinated deploy.

## Complexity Tracking

No constitution violations require justification.
