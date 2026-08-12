# Implementation Plan: Quản lý Applications

**Branch**: `002-manage-applications` | **Date**: 2026-08-11 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/002-manage-applications/spec.md`

## Summary

Thay trang chào `/applications` bằng màn hình CRUD có lọc, sắp xếp và phân trang. Backend áp dụng CQRS theo pattern hiện hữu: MediatR vertical slices, Dapper read repository cho search/detail và EF Core write repository cho create/update/soft-delete. Bổ sung optimistic concurrency `Version`, audit mọi mutation, kiểm tra dependency trước soft-delete, bốn permission policy độc lập và API contract nhất quán với Users. Frontend tạo feature Applications bằng React/MUI, giữ kết quả cũ khi reload lỗi và chỉ hiển thị hành động đúng capability.

## Technical Context

**Language/Version**: C# / .NET 10; JavaScript ES modules / React 19  
**Primary Dependencies**: ASP.NET Core, MediatR 14.2, FluentValidation 12.1, EF Core/MySql.EntityFrameworkCore 10.0.7, Dapper 2.1; React 19, Vite 8, Material UI 9  
**Storage**: MySQL; existing `applications` plus migration adding `Version` and explicit case-insensitive Code collation  
**Testing**: xUnit backend unit/integration/contract; Vitest + Testing Library/jsdom frontend; oxlint, Prettier and build gates  
**Target Platform**: ASP.NET Core web API and modern desktop/mobile web browsers supported by the client  
**Project Type**: Web application with layered backend and React SPA  
**Performance Goals**: With up to 10,000 Applications, 95% of list/filter/sort/page interactions visible within 2 seconds under normal load  
**Constraints**: CQRS; fail-closed authorization; parameterized SQL; no direct client database access; soft-delete; optimistic concurrency; non-destructive reload errors; no new dependency  
**Scale/Scope**: One screen, five endpoints, four permission policies, one schema migration, backend/frontend tests

## Constitution Check

*GATE: Passed before research and re-checked after design.*

| Principle / gate | Design evidence | Status |
|---|---|---|
| I. Security and Identity First | Authenticated named policies; server validation; parameterized persistence; safe logs/problems. | PASS |
| II. Enforced Layer Boundaries | Domain entity; Application use cases/interfaces; Infrastructure Dapper/EF; API transport/auth; client view/API separation. | PASS |
| III. Explicit API Contracts | OpenAPI defines shapes, authentication and errors; client handles all HTTP states. | PASS |
| IV. Tests Are Release Gates | Unit, persistence, API auth/contract and frontend tests plus commands in quickstart. | PASS |
| V. Consistent and Accessible UX | Reuse MUI/shared conventions with keyboard, focus, responsive and state feedback. | PASS |
| VI. Readable Multi-line Code | Formatter/lint and manual nested JSX/object formatting review required. | PASS |
| Migration governance | Version/collation migration includes representative validation and recovery guidance. | PASS |
| Simplicity/scope | Existing projects/dependencies reused; no unrelated refactor. | PASS |

### Post-design re-check

The data model, contract and quickstart preserve every gate. Permission policies satisfy independent rights; `Version` closes concurrency; explicit dependency checks close the soft-delete integrity gap. No constitution exception is required.

## Project Structure

### Documentation (this feature)

```text
specs/002-manage-applications/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── applications.openapi.yaml
└── tasks.md                         # Created later by $speckit-tasks
```

### Source Code (repository root)

```text
Identity-api/
├── src/
│   ├── Identity.Domain/Entities/AuthorizationEntities.cs
│   ├── Identity.Application/
│   │   ├── Abstractions/Persistence/{IApplicationsReadRepository,IApplicationsRepository}.cs
│   │   ├── Applications/{Dtos,GetApplications,GetApplicationById,CreateApplication,UpdateApplication,DeleteApplication}/
│   │   └── Common/Authorization/
│   ├── Identity.Infrastructure/
│   │   ├── Persistence/{DapperApplicationsReadRepository,MySqlApplicationsRepository}.cs
│   │   ├── Persistence/Configurations/SchemaConfigurations.cs
│   │   └── Migrations/
│   └── Identity.Api/
│       ├── Controllers/ApplicationsController.cs
│       ├── Authorization/
│       └── Program.cs
└── tests/
    ├── Identity.Application.Tests/Applications/
    ├── Identity.Infrastructure.IntegrationTests/Applications/
    └── Identity.Api.IntegrationTests/Applications/

Identity-client/
├── src/
│   ├── App.jsx
│   └── features/applications/
│       ├── ApplicationsPage.jsx
│       ├── index.js
│       ├── api/applicationsApi.js
│       └── components/{ApplicationsFilters,ApplicationsTable,ApplicationFormDialog,DeleteApplicationDialog}.jsx
└── tests/applications/
```

**Structure Decision**: Preserve the four-layer backend and feature-oriented client. Mirror Users where sound, while adding the permission, concurrency and audit guarantees required here. Extract a shared authenticated HTTP helper only if both features consume it without behavior changes.

## Implementation Design

### CQRS and persistence

- `GetApplications` and `GetApplicationById` depend on `IApplicationsReadRepository`; Dapper returns DTO projections, parameterizes filters, allow-lists sorts and excludes deleted rows.
- Create/Update/Delete depend on `IApplicationsRepository`; EF Core performs transactional writes and maps unique/concurrency failures to safe exceptions.
- Create trims fields, defaults active/not-deleted/Version 1 and stamps actor/time.
- Update requires current Version, changes all editable fields including `IsActive`, increments once and stamps actor/time.
- Delete requires Version, checks dependencies in the command flow, then sets deleted/inactive, increments Version and stamps actor/time; physical deletion is prohibited.
- Uniqueness includes deleted rows to match the unfiltered unique index. Explicit case-insensitive collation and database error mapping protect against races.

### Authorization and errors

- Require a valid authenticated session for every Applications endpoint; all authenticated users may view and perform CRUD operations.
- Search, detail, create, update and delete all require authentication. The client exposes the complete CRUD interface whenever a valid session exists; the server remains authoritative.
- Success uses `ApiResponse<T>`; validation 400, absent/deleted 404, duplicate/stale/dependency 409, normalized auth failures 401/403, and safe 500 use Problem Details.

### Client behavior

- Render `ApplicationsPage` at existing `/applications`; gate menu/direct access and actions from session permissions.
- Track draft/applied filters, paging/sorting, current result, loading, non-destructive error, dialogs, pending mutation, errors/notices and reload.
- Failed reload retains prior rows and offers Retry. Success reloads. Stale 409 preserves context and offers reload; dependency 409 leaves data unchanged.
- Dialogs trap/restore focus, label required/errors, disable repeat submission and support responsive widths.

## Complexity Tracking

No constitution violations require justification.
