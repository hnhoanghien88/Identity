# Implementation Plan: Quản lý Roles

**Branch**: `006-manage-roles` | **Date**: 2026-08-12 | **Spec**: [spec.md](spec.md)

## Summary

Bổ sung CRUD Roles có lọc, sắp xếp, phân trang và chọn Application. Backend mirror CQRS Actions: Dapper read, EF Core write, MediatR/FluentValidation; thêm `Version`, uniqueness `(ApplicationId, Code)` không phân biệt hoa/thường, xóa vật lý có bảo vệ Role hệ thống và quan hệ User/Permission. API chỉ yêu cầu đăng nhập. Frontend React/MUI thêm `/roles`.

## Technical Context

**Language/Version**: C# / .NET 10; JavaScript ES modules / React 19  
**Primary Dependencies**: ASP.NET Core, MediatR, FluentValidation, EF Core/MySQL, Dapper; React, Vite, Material UI  
**Storage**: MySQL `roles`; thêm Version; giữ ApplicationId, Code, Name, IsSystemRole, IsActive, audit  
**Testing**: xUnit; Vitest + Testing Library; oxlint, Prettier, production build  
**Target Platform**: ASP.NET Core API and modern browsers  
**Project Type**: Layered web API and React SPA  
**Performance Goals**: 10.000 Roles, 95% list/filter/sort/page trong 2 giây  
**Constraints**: CQRS; authenticated-only; không Roles policies; parameterized SQL; concurrency; dependency-safe delete; no new dependency  
**Scale/Scope**: One screen, five endpoints, one migration, backend/frontend tests

## Constitution Check

*GATE: Passed before research and re-checked after design.*

| Principle / gate | Evidence | Status |
|---|---|---|
| Security and identity | `[Authorize]`, validation, parameterized SQL, safe conflicts. | PASS |
| Layer boundaries | Domain, Application, Infrastructure/API and client feature boundaries. | PASS |
| Explicit contracts | OpenAPI covers shapes, authentication and errors. | PASS |
| Tests are gates | Unit, API, persistence, frontend, lint/build coverage. | PASS |
| Accessible UX | MUI patterns, keyboard/focus/responsive/loading/error. | PASS |
| Readable formatting | Formatter/lint and manual nested JSX review. | PASS |
| Migration governance | Duplicate precheck, recovery and coordinated rollback. | PASS |
| Simplicity | Mirrors Actions without new dependencies/policies. | PASS |

### Post-design re-check

Data model, contract and quickstart retain every gate. Authentication without Roles claims satisfies fail-closed security and no-permission access. No exception is required.

## Project Structure

```text
specs/006-manage-roles/{plan,research,data-model,quickstart,tasks}.md
specs/006-manage-roles/contracts/roles.openapi.yaml
Identity-api/src/Identity.Application/Roles/
Identity-api/src/Identity.Application/Abstractions/Persistence/{IRolesReadRepository,IRolesRepository}.cs
Identity-api/src/Identity.Infrastructure/Persistence/{DapperRolesReadRepository,MySqlRolesRepository}.cs
Identity-api/src/Identity.Api/Controllers/RolesController.cs
Identity-api/tests/*/Roles/
Identity-client/src/features/roles/
Identity-client/tests/roles/
```

**Structure Decision**: Preserve four backend layers and client feature folders; mirror Actions with Application lookup, Role flags and two dependency checks.

## Implementation Design

- Dapper uses parameterized filters, allow-listed sorts and paging; EF commands use validation/concurrency.
- Add unsigned `Version` default 1; update/delete require matching Version and update increments once.
- Delete blocks `IsSystemRole`, `user_roles`, `role_permissions`; FK races map to conflict.
- Controller uses `[Authorize]` with no Roles policies; ApiResponse and Problem Details remain consistent.
- Client adds `/roles`, session-visible menu, Application selector and Code/Name/System/Active form.
- Migration prechecks duplicate normalized Code per Application and documents recovery.

## Complexity Tracking

No constitution violations require justification.
